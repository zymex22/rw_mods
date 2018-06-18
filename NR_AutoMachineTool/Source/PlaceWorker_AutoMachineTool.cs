﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using NR_AutoMachineTool.Utilities;
using static NR_AutoMachineTool.Utilities.Ops;

namespace NR_AutoMachineTool
{
    public class PlaceWorker_AutoMachineTool : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            center.GetThingList(Find.CurrentMap)
                .Where(t => t.def == def)
                .FirstOption()
                .Select(b => b as Building_AutoMachineTool)
                .Select(b => b.OutputZone()
                    .Select(c => new { Cell = c, Color = Color.green })
                    .ToList()
                    .Append(new { Cell = b.OutputCell(), Color = Color.blue })
                    .Append(b.IngredientScanCell().Select(c => new { Cell = c, Color = Color.white }).ToList())
                )
                .GetOrDefaultF(() => GenAdj.CellsAdjacent8Way(center, rot, new IntVec2(1, 1)).ToList().Append(center).Select(c => new { Cell = c, Color = Color.white }).ToList())
                .GroupBy(a => a.Color)
                .ForEach(g => GenDraw.DrawFieldEdges(g.Select(b => b.Cell).ToList(), g.Key));
        }

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            var r = base.AllowsPlacing(checkingDef, loc, rot, map, thingToIgnore);
            if (r.Accepted)
            {
                if ((loc + rot.FacingCell).GetThingList(map)
                    .Where(t => t.def.category == ThingCategory.Building)
                    .SelectMany(t => Option(t as Building_WorkTable))
                    .Where(b => b.InteractionCell == loc).Count() == 0)
                {
                    return new AcceptanceReport("NR_AutoMachineTool.PlaceNotAllowed".Translate());
                }
                return r;
            }
            else
            {
                return r;
            }
        }
    }
}
