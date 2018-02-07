﻿# Rimworld 自動工作 MOD
- S.A.L.: Auto-crafters 2.0 を参考に作られています。(http://steamcommunity.com/sharedfiles/filedetails/?id=932193652)
- S.A.L.: Extra Crafters を参考に作られています。(http://steamcommunity.com/sharedfiles/filedetails/?id=940984361)
- Industrial Rollers を参考に作られています。(http://steamcommunity.com/sharedfiles/filedetails/?id=784327493)  
- Project RimFactory を参考に作られています。(https://steamcommunity.com/sharedfiles/filedetails/?id=1206316724)  
これらのMODの製作者様に感謝します。

I'm Japanese. I can't understand English, sorry...  
[machine translation](/NR_AutoMachineTool/README_en.md)

---
## 自動工作機械

![自動工作機械](/NR_AutoMachineTool/About/image01.png)

### 使い方
- 自動工作機械の研究をします。
- 自動工作機械をワークテーブルに面して設置します。
- 完成品の出力先を設定します。
- 自動工作機械はワークテーブルの作業依頼の中から、周囲8方向にある材料で作業可能な依頼を実行します。

### 機能
- 出力先の指定ができます。出力先が備蓄ゾーンで完成品が配置可能な場合には、出力先が含まれる備蓄ゾーン内のどこかに配置されます。
- 出力先が備蓄ゾーンで完成品が配置不可でも、出力先に指定したセルには配置されます。
- 自動工作機械を選択し、電力タブから電力供給量の変更ができます。電力供給量が大きい場合には、作成速度が上がります。
- Tierが上がると、スキルレベルが高い自動クラフターを作成できます。
- Tier毎の電力供給量の上限と下限やスキルレベルや速度の係数をMod設定画面にて設定できます。

### 注意
- 完成後に配置する場所がない場合には、配置待ちで処理が止まります。完成品をベルトコンベア等で移動させるか、出力先を備蓄ゾーンにすれば防げます。
- 作業中にワークテーブルや自動工作機械を破壊すると材料が失われる可能性があります。
- バランスは、良くないと思います。
- 対象のワークテーブルの加工依頼がある状態でMODを削除した場合、セーブデータが破損します。加工依頼をすべて取り消すか自動工作機を解体してから削除する事によって避ける事ができます。

### TODO
- バランス調整。
- MOD削除時の対応。

---
## ベルトコンベア

![ベルトコンベア](/NR_AutoMachineTool/About/image02.png)  
![地下コンベア](/NR_AutoMachineTool/About/image07.png)

### 使い方
- ベルトコンベアの研究をします。
- 運搬したい方向に向けて設置します。
- 上にアイテムを配置します。(アイテム引き出し器や自動工作機を利用できます。)
- ベルトコンベア同士は向きを垂直に隣り合って配置すると接続します。
- 地下コンベアは地下コンベア入り口と出口に接続します。

### 機能
- ベルトコンベア同士が接続され、出力先が複数ある場合には、フィルタタブから出力先毎にフィルターの設定が可能です。
- 出力先が備蓄ゾーンでアイテムが配置可能な場合には、出力先が含まれる備蓄ゾーン内のどこかに配置されます。
- 出力先が備蓄ゾーンでアイテムが配置不可でも、出力先に指定したセルには配置されます。
- 壁の中に配置できるベルトコンベアがあります。
- ベルトコンベアを選択し、電力タブから電力供給量の変更ができます。電力供給量が大きい場合には、移送速度が上がります。

### 注意
- 配置できる場所がない場合には、アイテムが詰まります。
- ベルトコンベア上のアイテムは入植者は関われません。
- バランスは、良くないと思います。
- ベルトコンベアを選択している状態では、地下コンベアが見えます。

### TODO
- バランス調整。

---
## アイテム引き出し機

![アイテム引き出し機](/NR_AutoMachineTool/About/image03.png)

### 使い方
- ベルトコンベアの研究をします。
- 引き出し元のゾーンに隣接させます。設置された向きの逆側から引き出し、設置された向きに出力します。
- 引き出し機は、設置後の状態では非稼働状態のため、引き出し機を選択し、ボタンより稼働状態にします。

### 機能
- フィルタタブから引き出すアイテムのフィルタを指定する事ができます。
- 出力先が備蓄ゾーンでアイテムが配置可能な場合には、出力先が含まれる備蓄ゾーン内のどこかに配置されます。
- 出力先が備蓄ゾーンでアイテムが配置不可でも、出力先に指定したセルには配置されます。
- 壁の中に配置できる引き出し機があります。


### 注意
- 出力先がベルトコンベアの場合でアイテムがすでに存在する場合、アイテム配置待ちになります。
- 出力先がベルトコンベアでない場合でアイテムがすでに存在する場合、出力先の付近に配置されます。
- バランスは、良くないと思います。

### TODO
- 引き出し制限

---
## 種蒔き機

![種蒔き機](/NR_AutoMachineTool/About/image05.png)

### 使い方
- 自動農業の研究をします。
- 農業ゾーンの内部か付近に設置します。
- 範囲内の農業ゾーンに、植物を植えていきます。

### 機能
- 種蒔き機を選択し、電力タブから電力供給量の変更ができます。電力供給量が大きい場合には、作成速度や種蒔き範囲が上がります。
- Tierが上がると、スキルレベルが高い種蒔き機を作成できます。
- 種蒔き機を選択し、制限タブから生産物の在庫数による処理の制限ができます。

### 注意
- 気温等で植える事ができない場所には植えません。
- 農業ゾーンに指定されていない植物があっても、刈り取りは行いません。
- 配置時の枠は電力最小時の枠です。設置後に電力を設定する事によって枠が広がります。
- バランスは、良くないと思います。

### TODO

---
## 収穫機

![収穫機](/NR_AutoMachineTool/About/image06.png)

### 使い方
- 自動農業の研究をします。
- 農業ゾーンの内部か付近に設置します。
- 範囲内の農業ゾーンから、収穫可能な植物を収穫します。

### 機能
- 収穫機を選択し、電力タブから電力供給量の変更ができます。電力供給量が大きい場合には、作成速度や収穫範囲が上がります。
- 出力先が備蓄ゾーンでアイテムが配置可能な場合には、出力先が含まれる備蓄ゾーン内のどこかに配置されます。
- 出力先が備蓄ゾーンでアイテムが配置不可でも、出力先に指定したセルには配置されます。
- Tierが上がると、スキルレベルが高い収穫機を作成できます。
- 収穫機を選択し、制限タブから生産物の在庫数による処理の制限ができます。

### 注意
- 出力先がベルトコンベアの場合でアイテムがすでに存在する場合、アイテム配置待ちになります。
- 出力先がベルトコンベアでない場合でアイテムがすでに存在する場合、出力先の付近に配置されます。
- 農業ゾーンに指定されていない植物があっても、収穫可能な場合には収穫します。
- 配置時の枠は電力最小時の枠です。設置後に電力を設定する事によって枠が広がります。
- バランスは、良くないと思います。

### TODO

---
## 動物世話機

![動物世話機](/NR_AutoMachineTool/About/image08.png)

### 使い方
- 自動動物飼育の研究をします。
- 動物が通過したり、滞在する付近に設置します。
- 動物世話機が範囲内の動物から、搾乳や毛刈りを行います。

### 機能
- 動物世話機を選択し、電力タブから電力供給量の変更ができます。電力供給量が大きい場合には、作成速度や収穫範囲が上がります。
- 出力先が備蓄ゾーンでアイテムが配置可能な場合には、出力先が含まれる備蓄ゾーン内のどこかに配置されます。
- 出力先が備蓄ゾーンでアイテムが配置不可でも、出力先に指定したセルには配置されます。
- 動物世話機を選択し、制限タブから生産物の在庫数による処理の制限ができます。

### 注意
- 出力先がベルトコンベアの場合でアイテムがすでに存在する場合、アイテム配置待ちになります。
- 出力先がベルトコンベアでない場合でアイテムがすでに存在する場合、出力先の付近に配置されます。
- 配置時の枠は電力最小時の枠です。設置後に電力を設定する事によって枠が広がります。
- バランスは、良くないと思います。

### TODO

---
## 屠畜機

![屠畜機](/NR_AutoMachineTool/About/image09.png)

### 使い方
- 自動動物飼育の研究をします。
- 動物が通過したり、滞在する付近に設置します。
- 屠畜機を選択し、設定タブから屠畜の設定を行います。
- 屠畜機が範囲内の動物の屠畜を行います。

### 機能
- 屠畜機を選択し、電力タブから電力供給量の変更ができます。電力供給量が大きい場合には、作成速度や収穫範囲が上がります。
- 出力先が備蓄ゾーンでアイテムが配置可能な場合には、出力先が含まれる備蓄ゾーン内のどこかに配置されます。
- 出力先が備蓄ゾーンでアイテムが配置不可でも、出力先に指定したセルには配置されます。
- 屠畜機を選択し、設定タブから残す家畜の数や屠畜の可否を設定できます。

### 注意
- 出力先がベルトコンベアの場合でアイテムがすでに存在する場合、アイテム配置待ちになります。
- 出力先がベルトコンベアでない場合でアイテムがすでに存在する場合、出力先の付近に配置されます。
- 配置時の枠は電力最小時の枠です。設置後に電力を設定する事によって枠が広がります。
- バランスは、良くないと思います。

### TODO

---
## 全体

### TODO
- デバッグ
- 画像を何とかする。。。
- 英語を何とかしたい。。。

### 注意  
当MODの再配布を禁じます。  