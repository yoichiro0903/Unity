//
//  BluetoothConnection.h
//  curtainWhetherApp
//
//  Created by WatanabeYoichiro on 2016/02/13.
//  Copyright © 2016年 YoichiroWatanabe. All rights reserved.
//

#import <Foundation/Foundation.h>
//CoreBluetoothのライブラリを読み込み。
#import <CoreBluetooth/CoreBluetooth.h>

//事前に調べた（Peripheralを自分で作った場合は自分で設定した）UUIDを定義。
#define kBatteryServiceUUID     @"180F" //これは割りとどのBluetooth機器でも共通らしい。
#define kBatteryCharUUID        @"2A19"
#define kScratch1ServiceUUID    @"A495FF20-C5B1-4B44-B512-1370F02D74DE"
#define kScratch1CharUUID       @"A495FF21-C5B1-4B44-B512-1370F02D74DE"
#define kAdServiceUUID          @"A495FF10-C5B1-4B44-B512-1370F02D74DE"

//デリゲートメソッドを宣言。
@protocol bluetoothNotificationDelegate <NSObject>
-(void)BTStateMassageSend:(NSString *)scanningState;
@end


@interface BluetoothConnection : NSObject
@property(nonatomic) BluetoothConnection *BTConnection;

//シングルトンのオブジェクトを宣言。
+ (id)sharedManager;

//セントラルマネージャのプロパティ宣言。
@property(nonatomic)BOOL isCentralBluetoothPoweredOn;
@property(nonatomic)BOOL isCentralScanning;
@property(nonatomic)BOOL isCentralConnectedToPeripheral;

@property(nonatomic)NSString *beanAccel;

//ペリフェラルのプロパティを宣言。
@property(nonatomic)int peripheralBatteryLevel;
@property(nonatomic)int peripheralScratch1Data;

//デリゲートプロパティを宣言。
//デリゲートプロパティを使用する際には、BTNotificationというプロパティ名を使用する必要がある。
@property(nonatomic, assign)id <bluetoothNotificationDelegate> BTNotification;
//他クラス（主にViewController）から呼び出すメソッドを宣言。
-(void)startScanning;
-(void)stopScanning;
-(void)disconnect;
-(void)disconnectIntrinsic;
-(void)changeScratch1Characteristics:(NSInteger *)state;
-(int)getBluetoothConnectionState;
-(void)callNotificationDelegateMethod:(NSString *)notificationMessage;
-(NSString *)getAccel;
@end

//CoreBluetoothクラスには接続に関する諸々のデリゲートメソッドが用意されていて、それらを使用する。
//その際に必要になるCoreBluetooth用の変数を宣言。
@interface BluetoothConnection() <CBCentralManagerDelegate, CBPeripheralDelegate>
@property(nonatomic,strong)  CBCentralManager *centralManager;
@property(nonatomic,strong)  CBPeripheral *peripheral;
@property(nonatomic,strong)  CBUUID *batteryServiceUUID;
@property(nonatomic,strong)  CBUUID *batteryServiceCharacteristicsUUID;
@property(nonatomic,strong)  CBUUID *scratch1ServiceUUID;
@property(nonatomic,strong)  CBUUID *scratch1ServiceCharacteristicsUUID;
@property(nonatomic,strong)  CBUUID *adServiceUUID;
@property(nonatomic,strong)  CBCharacteristic *batteryServiceCharacteristics;
@property(nonatomic,strong)  CBCharacteristic *scratch1ServiceCharacteristics;
@end