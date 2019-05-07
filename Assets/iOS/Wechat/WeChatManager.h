//
//  WeChatManager.h
//  Unity-iPhone
//
//  Created by 赵之韵 on 15/6/8.
//
//

#import <Foundation/Foundation.h>
#import "WXApi.h"

@interface WeChatManager : NSObject<WXApiDelegate> {
    /*! @brief 当前执行微信用到的奖励code。
     */
    NSString *_wexinShareContent;
    
    /*! @brief 当前执行的是否是分享朋友圈。
     */
    BOOL _isCircle;
}

+ (WeChatManager *) instance;

- (void) wxPay: (NSString *) order;

- (void) processWxpayResult: (int) errCode;

- (void) handleWxShare:(NSString *)url
                imgUrl:(NSString *)imgUrl
               imgPath:(NSString *)imgPath
                 title:(NSString *)title
               content:(NSString *)content
              thumbUrl:(NSString *)thumbUrl
             awardCode:(NSString *)awardCode
               toScene:(enum WXScene) scene;

@end
