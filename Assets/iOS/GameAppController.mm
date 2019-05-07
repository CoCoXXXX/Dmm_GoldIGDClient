#import "UnityAppController.h"
#import <StoreKit/StoreKit.h>
#import "WXApi.h"
#import "WeChatManager.h"
#import "IapManager.h"

@interface GameAppController : UnityAppController
@end

@implementation GameAppController

static NSString *wxAppId = @"wxd7dfb81088f364f1";

- (BOOL) application: (UIApplication*) application didFinishLaunchingWithOptions: (NSDictionary*) launchOptions {
    [super application: application didFinishLaunchingWithOptions: launchOptions];
    
    [WXApi registerApp: wxAppId];
    
    [[SKPaymentQueue defaultQueue] addTransactionObserver: [IapManager instance]];
    
    NSLog(@"didFinishLaunching, add IapManager: %p", [IapManager instance]);
    return YES;
}

- (BOOL) application: (UIApplication*) application
             openURL: (NSURL*) url
   sourceApplication: (NSString*) sourceApplication
          annotation: (id) annotation {
    [super application:application
               openURL:url
     sourceApplication:sourceApplication
            annotation:annotation];
    NSLog(@"openUrl:%@, sourceApplication:%@", url, sourceApplication);
    
    NSString *pay = [NSString stringWithFormat:@"%@://pay", wxAppId];
    if ([url.absoluteString containsString: pay]) {
        NSArray *array = [url.absoluteString componentsSeparatedByString:@"ret="];
        int ret = [array.lastObject intValue];
        [[WeChatManager instance] processWxpayResult:ret];
    } else {
        [WXApi handleOpenURL:url delegate:[WeChatManager instance]];
    }
    return YES;
}

- (void)applicationWillTerminate:(UIApplication*)application {
    [[SKPaymentQueue defaultQueue] removeTransactionObserver: [IapManager instance]];
    NSLog(@"applicationWillTerminate, remove IapManager: %p", [IapManager instance]);
    [super applicationWillTerminate:application];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(GameAppController)
