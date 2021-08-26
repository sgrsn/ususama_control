# UsusamaSerial

## 実行画面

![MainWindow](https://github.com/sgrsn/ususama_control/tree/feature/OpenRCF/images/MainWindow.png)

- Connect

マイコン(mbed)とのシリアル通信を行うオブジェクトを生成します

- Read Start

マイコンから送信されるデータを受信するタイマ割り込みを設定します。

- Step1～Step5

Step1から順に押していくとタスクを実行していきます

- Stop

停止命令を送信します。
再開はMoveで行います。


# Firmware

## mbed

https://os.mbed.com/users/sgrsn/code/UsusamaSerial/