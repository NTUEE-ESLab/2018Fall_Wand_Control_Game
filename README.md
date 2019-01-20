2018_Wand_Control_Game_Report
=============================

**動機**
--------

在各大平台上有很多節奏遊戲，但是都是利用手指在螢幕或鍵盤上操作的居多，我們想要利用擊劍的方式去玩節奏遊戲，
如此一來，更有揮擊的實感，進一步地說，我們可以開發出捨棄鍵盤，完全只由主機跟揮擊棒建立而成的遊戲。

**架構**
--------
利用MPU6050測出來的值，Arduino可以辨別你揮擊的方向，亮起相應的LED燈，同時利用HC 05傳輸資料到主機去。
並利用RPi camera捕捉影像，辨別你揮擊的位置，
在運行遊戲之前必須先開啟藍芽裝置以及RPi，好在開啟遊戲後能夠順利連線，在揮擊桿上用一顆按鈕，他可以用來Reset
你揮擊的初始位置以及暫停遊戲。
當遊戲開始時，你往下揮動揮擊桿即可產生隨機的箭頭圖案，在他們抵達指定的揮擊區域間揮擊即可得分，揮擊區域會閃爍
表示揮擊成功。


**Rpi**
-------
首先利用Rpi Camera 及 OpenCV 捕捉我們在棍子上貼的亮綠色膠帶，並透過Contours等功能找出包住那塊區域的RotatedRect,並用BoundingBox找出長方形四個頂點的座標，並計算長方形中心點的像素座標位置，再將棒子的左右相對位置等資訊透過Socket傳給電腦。


**Arduino**
-----------
我們利用MPU6050(六軸加速度計)裝在Arduino上，利用他傳回來的六個值，計算出ypr(yall, pitch , roll)三個角度值，並透過判斷往上、下、左、右四個方向揮擊的角度，在一定時間內從基準值變化到大於某個特定的角度，則亮相對應的燈號，並利用hc-05傳出相對應的方向判斷值給電腦。首先，其中因為mpu6050從通電到穩定需要一段時間，因此我們讓通電後按下按鈕實行校正並閃爍四個led燈，並計算每幾筆資料之間的差距直到小於某個值則判斷為校準完成，燈暗。並且由於mpu6050需要一個起始座標的修正，因此我們設置校準完後可以在自己想要的基準方向上按下按鈕，及會修正到使用者前方為原點。

![My image](https://github.com/NTUEE-ESLab/2018Fall_Wand_Control_Game/blob/master/pictures/arduino_circuit.png)

**Unity**
---------
我們運用Unity做遊戲的界面，從遊戲運行開始一直都會在的gameobject是背景圖案、揮擊區域還有一個消失區域，在遊戲開始時會去確認主機
藍牙有沒有連結揮擊鍵的藍牙(HC 05)，以及發送傳輸需求給RPi，無論有沒有連線到，遊戲都會開始，
當遊戲啟動後，主要是運用thread去接收藍芽的資料，這樣就算沒有吃到資料也不會卡住，至於RPi通過網路傳輸的資料是利用Update時每一個frame
去接收資料改變揮擊桿的位置，如果沒有吃到資料，默認為上一次的位置。
遊戲開始時，畫面上會出現一個START圖案，要先等到按下按鈕確認六軸正常運行後，當收到向下揮即是確認，START會發亮並消失，遊戲會在五秒後開始。
當遊戲運行時，再按一次按鈕，可以暫停遊戲，此時只要向下揮即可再次開始遊戲。	

遊戲的箭頭會往四個方向隨機出現，而為了讓他模擬三維的運動，就是變越大時(看起來越靠近你時)，我讓他出現的時間越長動得越快，比較有逼真的感覺。
只要箭頭底部在框框內的話，你揮擊便可使得箭頭消失同時框框內會發亮表示你揮擊成功，當箭頭底部碰到框框底部時他就會消失，也代表你錯過它了。


**成果**
--------
**實體樣子：**
![My image](https://github.com/NTUEE-ESLab/2018Fall_Wand_Control_Game/blob/master/pictures/wand2.jpg)

**遊戲畫面：**
遊戲時，當箭頭移動到最前面的半透明屏幕時，依照對應跑過來的箭頭方向做出揮動，如果揮動正確屏幕及會亮綠色。

**before hit**                                        
![My image](https://github.com/NTUEE-ESLab/2018Fall_Wand_Control_Game/blob/master/pictures/game0.png)
**hit**
![My image](https://github.com/NTUEE-ESLab/2018Fall_Wand_Control_Game/blob/master/pictures/game2.png)

**遊戲示範**
[![](https://github.com/NTUEE-ESLab/2018Fall_Wand_Control_Game/blob/master/pictures/game0.png)][Video]

[Video]: https://www.youtube.com/watch?v=UmU_biJP6gc
