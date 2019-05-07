#import <Foundation/Foundation.h>

/**
 * Iap的结果码
 */
enum IapCode {
    IapSuccess = 0,
    IapFailProductInfo = -1,
    IapFailPayment = -2,
    IapFailTradeInvalid = -3,
    IapFailCannotPay = -4
};

@interface IapManager : NSObject <SKPaymentTransactionObserver, SKProductsRequestDelegate>

+ (IapManager *)instance;

- (void)getProductInfo:(NSSet *) productIds;

- (BOOL)isProductInfoOk;

- (void)iapPay:(NSString *)productId
    outTradeNo:(NSString *)outTradeNo;

@end
