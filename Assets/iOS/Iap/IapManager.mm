#import <StoreKit/StoreKit.h>
#import "IapManager.h"
#import "Util.h"

@implementation IapManager {
    NSMutableDictionary *products;
}

+ (IapManager *)instance {
    static IapManager *_instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _instance = [[IapManager alloc] init];
    });
    
    return _instance;
}

- (id) init {
    self = [super init];
    
    products = [[NSMutableDictionary alloc] init];
    return self;
}

/**
 * 执行实际的支付逻辑
 */
- (void)iapPay:(NSString *)productId
    outTradeNo:(NSString *)outTradeNo {
    
    if (productId == nil || outTradeNo == nil) {
        NSLog(@"iap pay fail, invalid parameter.");
        
        [self sendPayResult:IapFailTradeInvalid
                 outTradeNo:outTradeNo
                    receipt:nil];
        return;
    }
    
    if (![SKPaymentQueue canMakePayments]) {
        NSLog(@"iap pay fail, can not make payments.");
        
        [self sendPayResult:IapFailCannotPay
                 outTradeNo:outTradeNo
                    receipt:nil];
        return;
    }
    
    SKProduct *product = nil;
    @synchronized (self) {
        product = products[productId];
    }
    
    if (product == nil) {
        NSLog(@"iap pay fail, no product info.");
        
        [self sendPayResult:IapFailProductInfo
                 outTradeNo:outTradeNo
                    receipt:nil];
        return;
    }
    
    NSLog(@"iap start payment -> {productId:%@, outTradeNo:%@}",
          productId,
          outTradeNo);
    
    SKMutablePayment *payment = [SKMutablePayment paymentWithProduct:product];
    payment.quantity = 1;
    payment.applicationUsername = outTradeNo;
    [[SKPaymentQueue defaultQueue] addPayment:payment];
}

/**
 * 监听Iap交易的状态
 */
- (void)paymentQueue:(SKPaymentQueue *)queue
 updatedTransactions:(NSArray *)transactions {
    
    for (SKPaymentTransaction *trans in transactions) {
        switch (trans.transactionState) {
            case SKPaymentTransactionStatePurchased:
                [self transactionSuccess:trans];
                break;
                
            case SKPaymentTransactionStatePurchasing:
                // 正在支付中这个状态，会在刚刚发起支付，弹出用户名密码对话框的时候调用一次。
                break;
                
            case SKPaymentTransactionStateFailed:
                [self transactionFail:trans];
                break;
                
            case SKPaymentTransactionStateDeferred:
                // 这个状态应该是在出现家长监护的时候出现。
                break;
                
            case SKPaymentTransactionStateRestored:
                // 不存在不同设备间恢复的问题，因此直接结束当前的订单。
                [self transactionFinish:trans];
                break;
                
            default:
                // 不知名情况的订单，也直接结束掉。
                [self transactionFinish:trans];
                break;
        }
    }
}

/**
 * 交易成功。
 */
- (void)transactionSuccess:(SKPaymentTransaction *)trans {
    if (trans == nil) {
        return;
    }
    
    NSURL *receiptUrl = [[NSBundle mainBundle] appStoreReceiptURL];
    NSData *receipt = [NSData dataWithContentsOfURL:receiptUrl];
    NSString *receiptDataString = [receipt base64EncodedStringWithOptions:0];
    
    NSString *outTradeNo = trans.payment.applicationUsername;
    NSString *productId = trans.payment.productIdentifier;
    
    NSLog(@"iap pay success. productId:%@, outTradeNo:%@, receipt:%@",
          productId,
          outTradeNo,
          receiptDataString);
    
    [self sendPayResult:IapSuccess
             outTradeNo:outTradeNo
                receipt:receiptDataString];
    
    [[SKPaymentQueue defaultQueue] finishTransaction:trans];
}

/**
 * 交易失败
 */
- (void)transactionFail:(SKPaymentTransaction *)trans {
    
    if (trans == nil) {
        return;
    }
    
    NSString *outTradeNo = trans.payment.applicationUsername;
    NSString *productId = trans.payment.productIdentifier;
    
    NSLog(@"iap pay fail. productId:%@, outTradeNo:%@",
          productId,
          outTradeNo);
    
    // 向Unity发送支付失败的消息。
    [self sendPayResult:IapFailPayment
             outTradeNo:outTradeNo
                receipt:nil];
    
    [[SKPaymentQueue defaultQueue] finishTransaction:trans];
}

/**
 * 交易完成
 */
- (void)transactionFinish:(SKPaymentTransaction *)trans {
    
    if (trans == nil) {
        return;
    }
    
    NSString *outTradeNo = trans.payment.applicationUsername;
    NSString *productId = trans.payment.productIdentifier;
    
    NSLog(@"iap pay finish. productId:%@, outTradeNo:%@",
          productId,
          outTradeNo);
    
    [[SKPaymentQueue defaultQueue] finishTransaction:trans];
}

-(void)getProductInfo:(NSSet *) productIds {
    NSLog(@"get product info.");
    SKProductsRequest *req = [[SKProductsRequest alloc] initWithProductIdentifiers:productIds];
    req.delegate = self;
    [req start];
}

-(BOOL)isProductInfoOk {
    @synchronized (self) {
        return [products count] > 0;
    }
}

/**
 * 成功获取Product信息
 */
- (void)productsRequest:(SKProductsRequest *)request
     didReceiveResponse:(SKProductsResponse *)response {
    
    if (response.products == nil || response.products.count <= 0) {
        NSLog(@"request product info fail!");
        return;
    }
    
    @synchronized (self) {
        for (SKProduct *pro in response.products) {
            if (pro == nil) {
                continue;
            }
            
            products[pro.productIdentifier] = pro;
        }
    }
    
    NSLog(@"request product info success, receive count:%lu.", [products count]);
}

/**
 * 请求失败了
 */
- (void) request:(SKRequest *)request
didFailWithError:(NSError *)error {
    if ([request isKindOfClass:[SKProductsRequest class]]) {
        NSLog(@"request product info fail!");
        return;
    }
}

/**
 * 向Unity发送Iap支付的结果
 */
- (void)sendPayResult:(int)result
           outTradeNo:(NSString *)outTradeNo
              receipt:(NSString *)receipt {
    
    NSString * res = [NSString stringWithFormat:
                      @"{\"Result\":%d,\"OutTradeNo\":\"%@\",\"Receipt\":\"%@\"}",
                      result,
                      outTradeNo != nil ? outTradeNo : @"",
                      receipt != nil ? receipt : @""];
    
    UnitySendMessage("PayManager", "IapPayResult", [res UTF8String]);
}

// Unity调用接口。

@end

extern "C" {
    void iapPay(const char *productId, const char *outTradeNo) {
        [[IapManager instance] iapPay:[Util charToNSString:productId]
                           outTradeNo:[Util charToNSString:outTradeNo]];
    }
    
    void getProductInfo(const char* productIds[], int productIdCount) {
        if (productIds == NULL) {
            return;
        }
        
        NSMutableSet *set = [[NSMutableSet alloc] init];
        for (int i = 0; i < productIdCount; i++) {
            [set addObject:[Util charToNSString:productIds[i]]];
        }
        
        [[IapManager instance] getProductInfo:set];
    }
    
    bool isProductInfoOk() {
        return [[IapManager instance] isProductInfoOk];
    }
}
