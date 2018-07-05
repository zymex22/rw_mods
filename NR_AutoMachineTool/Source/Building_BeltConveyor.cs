﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using UnityEngine;
using NR_AutoMachineTool.Utilities;
using static NR_AutoMachineTool.Utilities.Ops;

namespace NR_AutoMachineTool
{
    class Building_BeltConveyor : Building_Base<Thing>, IBeltConbeyorLinkable
    {
        public Building_BeltConveyor()
        {
            base.setInitialMinPower = false;
        }

        private Rot4 dest = default(Rot4);
        private Dictionary<Rot4, ThingFilter> filters = new Dictionary<Rot4, ThingFilter>();
        public static float supplyPower = 10f;

        [Unsaved]
        private int round = 0;
        [Unsaved]
        private List<Rot4> outputRot = new List<Rot4>();

        public IEnumerable<Rot4> OutputRots => this.outputRot;

        private ModExtension_AutoMachineTool Extension { get { return this.def.GetModExtension<ModExtension_AutoMachineTool>(); } }

        protected override float SpeedFactor { get => this.Setting.beltConveyorSetting.speedFactor; }
        public override int MinPowerForSpeed { get => this.Setting.beltConveyorSetting.minSupplyPowerForSpeed; }
        public override int MaxPowerForSpeed { get => this.Setting.beltConveyorSetting.maxSupplyPowerForSpeed; }

        public override float SupplyPowerForSpeed
        {
            get
            {
                return supplyPower;
            }

            set
            {
                supplyPower = value;
                this.SetPower();
            }
        }

        public Dictionary<Rot4, ThingFilter> Filters { get => this.filters; }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<float>(ref supplyPower, "supplyPower", 10f);
            Scribe_Values.Look<Rot4>(ref this.dest, "dest");
            Scribe_Collections.Look<Rot4, ThingFilter>(ref this.filters, "filters", LookMode.Value, LookMode.Deep);
            if(this.filters == null)
            {
                this.filters = new Dictionary<Rot4, ThingFilter>();
            }
        }

        public override void PostMapInit()
        {
            base.PostMapInit();
            
            this.FilterSetting();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (!respawningAfterLoad)
            {
                LinkTargetConveyor().ForEach(x =>
                {
                    x.Link(this);
                    this.Link(x);
                });
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            var targets = LinkTargetConveyor();

            this.Reset();

            base.DeSpawn();

            targets.ForEach(x => x.Unlink(this));
        }

        protected override void Reset()
        {
            if (this.State != WorkingState.Ready)
            {
                this.FilterSetting();
            }
            base.Reset();
        }
        
        public override void DrawGUIOverlay()
        {
            base.DrawGUIOverlay();

            if (this.IsUnderground && !OverlayDrawHandler_UGConveyor.ShouldDraw)
            {
                // 地下コンベアの場合には表示しない.
                return;
            }

            if (this.State != WorkingState.Ready && Find.CameraDriver.CurrentZoom == CameraZoomRange.Closest)
            {
                var p = CarryPosition();
                Vector2 result = Find.Camera.WorldToScreenPoint(p + new Vector3(0, 0, -0.4f)) / Prefs.UIScale;
                result.y = (float)UI.screenHeight - result.y;
                GenMapUI.DrawThingLabel(result, this.CarryingThing().stackCount.ToStringCached(), GenMapUI.DefaultThingLabelColor);
            }
        }

        public override void Draw()
        {
            if (this.IsUnderground && !OverlayDrawHandler_UGConveyor.ShouldDraw)
            {
                // 地下コンベアの場合には表示しない.
                return;
            }
            base.Draw();
            if (this.State != WorkingState.Ready)
            {
                var p = CarryPosition();
                this.CarryingThing().DrawAt(p);
            }
        }

        private Thing CarryingThing()
        {
            if (this.State == WorkingState.Working)
            {
                return this.working;
            }
            else if (this.State == WorkingState.Placing)
            {
                return this.products[0];
            }
            return null;
        }

        private Vector3 CarryPosition()
        {
            return (this.dest.FacingCell.ToVector3() * (1f - this.workLeft)) + this.Position.ToVector3() + new Vector3(0.5f, 10f, 0.5f);
        }
        
        public override bool CanStackWith(Thing other)
        {
            return base.CanStackWith(other) && this.State == WorkingState.Ready;
        }

        public bool ReceiveThing(bool underground, Thing t)
        {
            return ReceiveThing(underground, t, Destination(t, true));
        }

        private bool ReceiveThing(bool underground, Thing t, Rot4 rot)
        {
            if (!this.ReceivableNow(underground, t))
                return false;
            if (this.State == WorkingState.Ready)
            {
                this.dest = rot;
                this.working = t;
                this.workLeft = 1f;
                if (this.working.Spawned) this.working.DeSpawn();
                this.State = WorkingState.Working;
                return true;
            }
            else
            {
                var target = this.State == WorkingState.Working ? this.working : this.products[0];
                return target.TryAbsorbStack(t, true);
            }
        }

        private Rot4 Destination(Thing t, bool doRotate)
        {
            var allowed = this.filters
                .Where(f => f.Value.Allows(t.def)).Select(f => f.Key)
                .ToList();
            var placable = allowed.Where(r => this.OutputBeltConveyor().Where(l => l.Position == this.Position + r.FacingCell).FirstOption().Select(b => b.ReceivableNow(this.IsUnderground, t)).GetOrDefault(true))
                .ToList();

            if (placable.Count == 0)
            {
                if(allowed.Count == 0)
                {
                    return this.Rotation;
                }
                placable = allowed;
            }

            if (placable.Count <= this.round) this.round = 0;
            var index = this.round;
            if (doRotate) this.round++;
            return placable.ElementAt(index);
        }

        private bool SendableConveyor(Thing t, out Rot4 dir)
        {
            dir = default(Rot4);
            var result = this.filters
                .Where(f => f.Value.Allows(t.def))
                .Select(f => f.Key)
                .SelectMany(r => this.OutputBeltConveyor().Where(l => l.Position == this.Position + r.FacingCell).Select(b => new { Dir = r, Conveyor = b }))
                .Where(b => b.Conveyor.ReceivableNow(this.IsUnderground, t))
                .FirstOption();
            if (result.HasValue)
            {
                dir = result.Value.Dir;
            }
            return result.HasValue;
        }

        protected override bool PlaceProduct(ref List<Thing> products)
        {
            var thing = products[0];
            var next = this.LinkTargetConveyor().Where(o => o.Position == this.dest.FacingCell + this.Position).FirstOption();
            if (next.HasValue)
            {
                // コンベアある場合、そっちに流す.
                if (next.Value.ReceiveThing(this.IsUnderground, thing))
                {
                    NotifyAroundSender();
                    return true;
                }
            }
            else
            {
                if (!this.IsUnderground && PlaceItem(thing, this.dest.FacingCell + this.Position, false, this.Map))
                {
                    NotifyAroundSender();
                    return true;
                }
            }

            var dir = default(Rot4);
            if (this.SendableConveyor(thing, out dir))
            {
                // 他に流す方向があれば、やり直し.
                this.Reset();
                this.ReceiveThing(this.IsUnderground, thing, dir);
                return false;
            }
            // 配置失敗.
            this.workLeft = 0.5f;
            return false;
        }

        public void Link(IBeltConbeyorLinkable link)
        {
            this.FilterSetting();
        }

        public void Unlink(IBeltConbeyorLinkable unlink)
        {
            this.FilterSetting();
            Option(this.working).ForEach(t => this.dest = Destination(t, true));
        }

        private void FilterSetting()
        {
            Func<ThingFilter> createNew = () =>
            {
                var f = new ThingFilter();
                f.SetAllowAll(null);
                return f;
            };
            var output = this.OutputBeltConveyor();
            this.filters = Enumerable.Range(0, 4).Select(x => new Rot4(x))
                .Select(x => new { Rot = x, Pos = this.Position + x.FacingCell })
                .Where(x => output.Any(l => l.Position == x.Pos) || this.Rotation == x.Rot)
                .ToDictionary(r => r.Rot, r => this.filters.ContainsKey(r.Rot) ? this.filters[r.Rot] : createNew());
            if(this.filters.Count <= 1)
            {
                this.filters.ForEach(x => x.Value.SetAllowAll(null));
            }
            this.outputRot = this.filters.Select(x => x.Key).ToList();
        }

        private List<IBeltConbeyorLinkable> LinkTargetConveyor()
        {
            return Enumerable.Range(0, 4).Select(i => this.Position + new Rot4(i).FacingCell)
                .SelectMany(t => t.GetThingList(this.Map))
                .Where(t => t.def.category == ThingCategory.Building)
                .Where(t => CanLink(this, t, this.def, t.def))
                .SelectMany(t => Option(t as IBeltConbeyorLinkable))
                .ToList();
        }

        private List<IBeltConbeyorLinkable> OutputBeltConveyor()
        {
            var links = this.LinkTargetConveyor();
            return links.Where(x =>
                    (x.Rotation.Opposite.FacingCell + x.Position == this.Position && x.Position != this.Position + this.Rotation.Opposite.FacingCell) ||
                    (x.Rotation.Opposite.FacingCell + x.Position == this.Position && links.Any(l => l.Position + l.Rotation.FacingCell == this.Position))
                )
                .ToList();
        }

        public bool Acceptable(Rot4 rot, bool underground)
        {
            return rot != this.Rotation && this.IsUnderground == underground;
        }

        public bool ReceivableNow(bool underground, Thing thing)
        {
            if(!this.IsActive() || this.IsUnderground != underground)
            {
                return false;
            }
            Func<Thing, bool> check = (t) => t.CanStackWith(thing) && t.stackCount < t.def.stackLimit;
            switch (this.State) {
                case WorkingState.Ready:
                    return true;
                case WorkingState.Working:
                    // return check(this.working);
                    return false;
                case WorkingState.Placing:
                    return check(this.products[0]);
                default:
                    return false;
            }
        }

        public bool IsUnderground { get => Option(this.Extension).Fold(false)(x => x.underground); }

        private void NotifyAroundSender()
        {
            new Rot4[] { this.Rotation.Opposite, this.Rotation.Opposite.RotateAsNew(RotationDirection.Clockwise), this.Rotation.Opposite.RotateAsNew(RotationDirection.Counterclockwise) }
                .Select(r => this.Position + r.FacingCell)
                .SelectMany(p => p.GetThingList(this.Map))
                .Where(t => t.def.category == ThingCategory.Building)
                .SelectMany(t => Option(t as IBeltConbeyorSender))
                .ForEach(s => s.NortifyReceivable());
        }

        protected override float GetTotalWorkAmount(Thing working)
        {
            return 1f;
        }

        protected override bool WorkIntrruption(Thing working)
        {
            return false;
        }

        protected override bool TryStartWorking(out Thing target)
        {
            if (this.IsUnderground)
            {
                target = null;
                return false;
            }
            target = this.Position.GetThingList(this.Map).Where(t => t.def.category == ThingCategory.Item).FirstOption().GetOrDefault(null);
            if (target != null)
            {
                this.ReceiveThing(this.IsUnderground, target);
            }
            return target != null;
        }

        protected override bool FinishWorking(Thing working, out List<Thing> products)
        {
            products = new List<Thing>().Append(working);
            return true;
        }

        protected override bool WorkingIsDespawned()
        {
            return true;
        }

        public static bool IsBeltConveyorDef(ThingDef def)
        {
            return typeof(Building_BeltConveyor).IsAssignableFrom(def.thingClass);
        }

        public static bool IsUndergroundDef(ThingDef def)
        {
            return Option(def.GetModExtension<ModExtension_AutoMachineTool>()).Fold(false)(x => x.underground);
        }


        public static bool CanLink(Thing @this, Thing other, ThingDef thisDef, ThingDef otherDef)
        {
            var t = @this;
            if (Building_BeltConveyor.IsBeltConveyorDef(thisDef))
            {
                var ug = Building_BeltConveyor.IsUndergroundDef(thisDef);
                if (Building_BeltConveyor.IsBeltConveyorDef(otherDef))
                {
                    return ug == Building_BeltConveyor.IsUndergroundDef(otherDef) && (
                        t.Position + t.Rotation.FacingCell == other.Position ||
                        t.Position + t.Rotation.Opposite.FacingCell == other.Position ||
                        other.Position + other.Rotation.FacingCell == t.Position ||
                        other.Position + other.Rotation.Opposite.FacingCell == t.Position);
                }
                else if (Building_BeltConveyorUGConnecter.IsConveyorUGConnecterDef(otherDef))
                {
                    return t.Position + t.Rotation.FacingCell == other.Position ||
                        (other.Position + other.Rotation.FacingCell == t.Position && ug == Building_BeltConveyorUGConnecter.ToUndergroundDef(otherDef)) ||
                        (other.Position + other.Rotation.Opposite.FacingCell == t.Position && ug != Building_BeltConveyorUGConnecter.ToUndergroundDef(otherDef));
                }
            }
            else if (Building_BeltConveyorUGConnecter.IsConveyorUGConnecterDef(thisDef))
            {
                var toUg = Building_BeltConveyorUGConnecter.ToUndergroundDef(thisDef);
                if (Building_BeltConveyor.IsBeltConveyorDef(otherDef))
                {
                    return (t.Position + t.Rotation.FacingCell == other.Position && toUg == Building_BeltConveyor.IsUndergroundDef(otherDef)) ||
                        (t.Position + t.Rotation.Opposite.FacingCell == other.Position && toUg != Building_BeltConveyor.IsUndergroundDef(otherDef));
                }
                else if (Building_BeltConveyorUGConnecter.IsConveyorUGConnecterDef(otherDef))
                {
                    return (t.Position + t.Rotation.FacingCell == other.Position && toUg != Building_BeltConveyorUGConnecter.ToUndergroundDef(otherDef)) ||
                        (t.Position + t.Rotation.Opposite.FacingCell == other.Position && toUg != Building_BeltConveyorUGConnecter.ToUndergroundDef(otherDef));
                }
            }
            return false;
        }
    }
}
