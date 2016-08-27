//
//  ViewController.m
//  curtainWhetherApp
//
//  Created by WatanabeYoichiro on 2016/02/13.
//  Copyright © 2016年 YoichiroWatanabe. All rights reserved.
//

#import "ViewControllerUnity.h"
#import <UIKit/UIKit.h>
#import "BluetoothConnection.h"

@interface ViewController ()

@end

@implementation ViewController

//char* MakeStringCopyBT (const char* string)
//{
//    if (string == NULL)
//        return NULL;
//    
//    char* res = (char*)malloc(strlen(string) + 1);
//    strcpy(res, string);
//    return res;
//}
//

//-(char*) BluetoothConnectionUnity_ {
//    _BTConnection = [BluetoothConnection sharedManager];
//    _BTConnection.BTNotification = self;
//    [_BTConnection startScanning];
//    NSString *message = @"ok";
//    return MakeStringCopyBT([message UTF8String]);
//}



//char *BluetoothConnectionUnity_ () {    
//    NSArray *languages = [NSLocale preferredLanguages];
//    NSString *currentLanguage = [languages objectAtIndex:0];
//    return MakeStringCopyBT([currentLanguage UTF8String]);
//}

@end