//
//  BluetoothConnection.m
//  goodMorningCurtainApp
//
//  Created by WatanabeYoichiro on 2016/02/13.
//  Copyright © 2016年 YoichiroWatanabe. All rights reserved.
//

#import "BluetoothConnection.h"
#import <CoreBluetooth/CoreBluetooth.h>

@implementation BluetoothConnection

char* MakeStringCopyBT (const char* string)
{
    if (string == NULL)
        return NULL;

    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

char* BluetoothConnectionUnity_ () {
    BluetoothConnection *_BTConnection = [BluetoothConnection sharedManager];
//    *_BTConnection.BTNotification = self;
    [_BTConnection startScanning];
    NSString *message = @"Initialized BTConnection";
    return MakeStringCopyBT([message UTF8String]);
}


// now unused
-(NSString *)getAccel {
    NSString *accelLog = self.beanAccel;
    NSLog(@"Bean Accel : %@", accelLog);
    return accelLog;
}


//シングルトンインスタンス生成。
+ (id)sharedManager {
    static id instance = nil;
    
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[self alloc] init];
        [instance initInstance];
    });
    NSLog(@"%s","sharedManager");
    return instance;
}

- (void)initInstance {
    //シングルトンインスタンスをself（BluetoothConnectionクラス）で生成し、
    //そこに紐づくプロパティへ、Core Bluetooth関連の変数を代入。
    if (self) {
        _centralManager = [[CBCentralManager alloc] initWithDelegate:self queue:nil];
        _batteryServiceUUID = [CBUUID UUIDWithString:kBatteryServiceUUID];
        _batteryServiceCharacteristicsUUID = [CBUUID UUIDWithString:kBatteryCharUUID];
        _scratch1ServiceUUID = [CBUUID UUIDWithString:kScratch1ServiceUUID];
        _scratch1ServiceCharacteristicsUUID = [CBUUID UUIDWithString:kScratch1CharUUID];
        //バックグラウンドでスキャンするときは、サービス指定が必要。それ用。
        _adServiceUUID = [CBUUID UUIDWithString:kAdServiceUUID];
        NSLog(@"%s","init instance");
    }
}

//ViewControllerから呼び出されて、scanForPeripheralsWithServicesを開始。
-(void)startScanning
{
    if (!self.isCentralBluetoothPoweredOn) {
        //[self callNotificationDelegateMethod:@"Bluetooth remains OFF."];
        NSLog(@"%@",self);
        NSLog(@"%s","Central BT is powerd off");
        return;
    }
    if (self.isCentralScanning) {
        //[self callNotificationDelegateMethod:@"Central is now Scannning. Please Wait."];
        NSLog(@"%s","Central BT already scanning");
        return;
    }
    NSDictionary *scanOptions = [NSDictionary dictionaryWithObject:[NSNumber numberWithBool:NO] forKey:CBCentralManagerScanOptionAllowDuplicatesKey];
    //バックグラウンドでスキャンしたい場合、scanするUUIDを指定する必要がある。
    //scanするUUIDを指定しない場合：[_centralManager scanForPeripheralsWithServices:nil options:scanOptions];
    NSArray *serviceUUIDArray = [NSArray arrayWithObjects:_adServiceUUID, nil];
    
    //ペリフェラルを_adServiceUUIDでスキャン開始。ペリフェラルが見つかると、didDiscoverPeripheralが実行される。
    [_centralManager scanForPeripheralsWithServices:serviceUUIDArray options:scanOptions];
    self.isCentralScanning = YES;
    NSLog(@"%s","Central started scanning...");
    //[self callNotificationDelegateMethod:@"Central started scanning..."];
}


-(void)disconnectIntrinsic
{
    [self stopScanning];
    self.isCentralScanning =NO;
    self.isCentralConnectedToPeripheral = NO;
    _peripheral = nil;
    _batteryServiceCharacteristics = nil;
    _scratch1ServiceCharacteristics = nil;
    self.peripheralBatteryLevel = 0;
    self.peripheralScratch1Data = 0;
    //[self callNotificationDelegateMethod:@"Central is disconnected"];
    NSLog(@"%s", "Central is disconnected");
}

-(void)stopScanning
{
    if (!_isCentralScanning) {
        return; //そもそもスキャンしていなかったら何もしない。
    }
    [_centralManager stopScan];
    self.isCentralScanning = NO;
}

-(void)disconnect
{
    if (_peripheral == nil) {
        return;
    }
    [_centralManager cancelPeripheralConnection:_peripheral];
}

-(CBCharacteristic *)findCharacteristics:(NSArray *)cs uuid:(CBUUID *)uuid
{
    for (CBCharacteristic *c in cs) {
        if ([c.UUID.data isEqualToData:uuid.data]) {
            return c;
        }
    }
    return nil;
}

//iphoneのBluetoothがどうなっているかを都度検知する。アプリが起動するとまず実行される。
-(void)centralManagerDidUpdateState:(CBCentralManager *)central
{
    switch ([_centralManager state]) {
        case CBCentralManagerStatePoweredOff:
            self.isCentralBluetoothPoweredOn = NO;
            self.isCentralScanning = NO;
            self.isCentralConnectedToPeripheral = NO;
//            NSLog(@"%ld", (long)[_centralManager state]);
            NSLog(@"CBCentralManagerStatePoweredOff");
            break;
        case CBCentralManagerStatePoweredOn:
            self.isCentralBluetoothPoweredOn = YES;
//            NSLog(@"%ld", (long)[_centralManager state]);
            NSLog(@"CBCentralManagerStatePoweredOn");
            [self startScanning]; //resume
            break;
        case CBCentralManagerStateResetting:
//            NSLog(@"%ld", (long)[_centralManager state]);
            NSLog(@"CBCentralManagerStateResetting");
            break;
        case CBCentralManagerStateUnauthorized:
//            NSLog(@"%ld", (long)[_centralManager state]);
            NSLog(@"CBCentralManagerStateUnauthorized");
            break;
        case CBCentralManagerStateUnknown:
//            NSLog(@"%ld", (long)[_centralManager state]);
            NSLog(@"CBCentralManagerStateUnknown");
            break;
        case CBCentralManagerStateUnsupported:
//            NSLog(@"%ld", (long)[_centralManager state]);
            NSLog(@"CBCentralManagerStateUnsupported");
            break;
    }
}



-(void)centralManager:(CBCentralManager *)central didDiscoverPeripheral:(CBPeripheral *)peripheral advertisementData:(NSDictionary<NSString *,id> *)advertisementData RSSI:(NSNumber *)RSSI
{
    if (_peripheral != nil) {
        NSLog(@"%s", "Already Discoverd peripheral.");
        return;
    }
    NSString *localName = [advertisementData objectForKey:CBAdvertisementDataLocalNameKey];
    if (localName != nil) {
        _peripheral = peripheral;
        
        NSLog(@"RSSI : %@",RSSI);
        NSLog(@"Device name is %@", localName);
        //didConnectPeripheralを呼び出す。
        //NSLog(@"Discovered %@", peripheral.name);という形で見つかったペリフェラルのリストを吐き出す。
        [central connectPeripheral:_peripheral options:nil];
        [self stopScanning];
    } else {
        NSLog(@"Device name is nil. localName:%@", localName);
    }
}

-(void)centralManager:(CBCentralManager *)central didConnectPeripheral:(CBPeripheral *)peripheral
{
    _peripheral.delegate = self;
    self.isCentralConnectedToPeripheral = YES;
    NSLog(@"did connect peripheral");
    //[self callNotificationDelegateMethod:@"Peripheral connected to Central."];
    //didDiscoverServicesを呼び出す。
    [peripheral discoverServices:[NSArray arrayWithObjects:_batteryServiceUUID, _scratch1ServiceUUID, nil]];
}

-(void)centralManager:(CBCentralManager *)central didFailToConnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    [self disconnectIntrinsic]; //接続に失敗したら、初期状態に戻す。
    NSLog(@"%s", "Failed to Connect Periperal");
}

-(void)centralManager:(CBCentralManager *)central didDisconnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    [self disconnectIntrinsic]; //接続を切断したら、初期状態に戻す。
    NSLog(@"%s", "Disconnected Periperal");
}

-(void)peripheral:(CBPeripheral *)peripheral didDiscoverServices:(NSError *)error
{
    //for debug
    //[NSThread sleepForTimeInterval:10];
    for (CBService *service in peripheral.services) {
        if ([service.UUID.data isEqualToData:_batteryServiceUUID.data]) {
            //_batteryServiceCharacteristicsUUIDをスキャンする。
            //didDiscoverCharacteristicsForServiceを呼び出す。
            [peripheral discoverCharacteristics:[NSArray arrayWithObjects:_batteryServiceCharacteristicsUUID, nil] forService:service];
            NSLog(@"%s", "Discovered battery service");
            //[self callNotificationDelegateMethod:@"Discovered battery service"];
        } else if ([service.UUID.data isEqualToData:_scratch1ServiceUUID.data]) {
            //_scratch1ServiceCharacteristicsUUIDをスキャンする。
            //didDiscoverCharacteristicsForServiceを呼び出す。
            [peripheral discoverCharacteristics:[NSArray arrayWithObjects:_scratch1ServiceCharacteristicsUUID, nil] forService:service];
            NSLog(@"%s", "Discovered scratch1 service");
            //[self callNotificationDelegateMethod:@"Discovered scratch1 service"];
        }
        NSLog(@"service : %@", service);
    }
}

-(void)peripheral:(CBPeripheral *)peripheral didDiscoverCharacteristicsForService:(CBService *)service error:(NSError *)error
{
    if ([service.UUID.data isEqualToData:_batteryServiceUUID.data]) {
        _batteryServiceCharacteristics = [self findCharacteristics:service.characteristics uuid:_batteryServiceCharacteristicsUUID];
        //didUpdateValueForCharacteristicを呼び出す。
        [peripheral readValueForCharacteristic:_batteryServiceCharacteristics];
        NSLog(@"%s", "Discovered battery characteristics");
    } else if ([service.UUID.data isEqualToData:_scratch1ServiceUUID.data]) {
        _scratch1ServiceCharacteristics = [self findCharacteristics:service.characteristics uuid:_scratch1ServiceCharacteristicsUUID];
        //didUpdateValueForCharacteristicを呼び出す。
        [peripheral readValueForCharacteristic:_scratch1ServiceCharacteristics];
        NSLog(@"%s", "Discovered scratch1 characteristics");
        //Characteristicsが変化したら都度通知するように申し込みする。
        [peripheral setNotifyValue:YES forCharacteristic:_scratch1ServiceCharacteristics];
    }
}

//こいつは通知申し込みの時に一度だけ呼びだされ、Characteristicsが変化した時にdidUpdateValueForCharacteristic都度呼び出すようになる。
-(void)peripheral:(CBPeripheral *)peripheral didUpdateNotificationStateForCharacteristic:(nonnull CBCharacteristic *)characteristic error:(nullable NSError *)error {
    if (error) {
        NSLog(@"Error changing notification state: %@", [error localizedDescription]);
    } else {
        NSLog(@"Changed characteristics : %@", characteristic);
    }
}

-(void)peripheral:(CBPeripheral *)peripheral didUpdateValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    uint8_t a;
    uint8_t b;
    if (characteristic == _batteryServiceCharacteristics) {
        [characteristic.value getBytes:&b length:1];
        self.peripheralBatteryLevel = b;
        NSLog(@"Battery level is %d" , b);
        //NSString *batteryLevelLog = [NSString stringWithFormat:@"Battery Level is %d",b];
        //[self callNotificationDelegateMethod:batteryLevelLog];
    } else if (characteristic == _scratch1ServiceCharacteristics){
        //something...
        _scratch1ServiceCharacteristics = characteristic;
        NSLog(@"Found Scratch Chara: %@", _scratch1ServiceCharacteristics);
        [characteristic.value getBytes:&a length:1];
        NSLog(@"Found Scratch Chara: %d", a);
        self.beanAccel = [NSString stringWithFormat:@"%d", a];
        UnitySendMessage("accelText", "recieveAccel", MakeStringCopyBT([self.beanAccel UTF8String]));
        NSLog(@"UnitySendMessage: %@", self.beanAccel);
        NSInteger state = 1;
        
        //バックグラウンドでcharacteristicの書き込みが出来るかの実験用。
        [self changeScratch1Characteristics:&state];
    }
}

//characteristicの書き込みが終わった時に呼び出される。
-(void)peripheral:(CBPeripheral *)peripheral didWriteValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    NSLog(@"%s","Send data to peripheral");
}

-(void)changeScratch1Characteristics:(NSInteger *)state
{
    //渡ってきた値を確かめる用。
//for accel test     NSLog(@"Got state value is : %ld", (long) *state);
    ushort value = *state;
    NSMutableData *data;
    if (!value) {
        value = 3;
        data = [NSMutableData dataWithBytes:&value length:2];
    } else {
        data = [NSMutableData dataWithBytes:&value length:2];
    }
    //ここでcharacteristicを書き込み。
//for accel test     [_peripheral writeValue:data forCharacteristic:_scratch1ServiceCharacteristics type:CBCharacteristicWriteWithResponse];
//for accel test     NSLog(@"Wrote to scratch1 value of : %d chara:%@", value, _scratch1ServiceCharacteristics);
    //NSString *scratch1WroteLog = [NSString stringWithFormat:@"Wrote to scratch1 value of %d", value];
    //[self callNotificationDelegateMethod:scratch1WroteLog];

}

-(int)getBluetoothConnectionState{
    int scanningState;
    if (self.isCentralScanning) {
        scanningState = 1;
        return scanningState;
    } else if (self.isCentralConnectedToPeripheral){
        scanningState = 2;
        return scanningState;
    } else {
        scanningState = 0;
        return scanningState;
    }
}


#pragma mark - Private delegate methods
-(void)callNotificationDelegateMethod:(NSString*)notificationMessage{
    NSString *scanningState = notificationMessage;
    //Check delegate method is exist or not
    if ([self.BTNotification respondsToSelector:@selector(BTStateMassageSend:)]) {
        [self.BTNotification BTStateMassageSend:scanningState];
        NSLog(@"BTStateMassageSend was called");
    }
}


@end