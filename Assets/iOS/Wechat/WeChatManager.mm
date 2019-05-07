//
//  WeChatManager.m
//  Unity-iPhone
//
//  Created by 赵之韵 on 15/6/8.
//
//

#import "WeChatManager.h"
#import "Util.h"

@implementation WeChatManager

+ (WeChatManager *)instance {
    static WeChatManager *_instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _instance = [[WeChatManager alloc] init];
    });
    
    return _instance;
}

#pragma mark - 微信登陆

- (void)wxAuth:(NSString *)deviceId {
    //构造SendAuthReq结构体
    SendAuthReq *req = [[SendAuthReq alloc] init];
    req.scope = @"snsapi_userinfo";
    req.state = deviceId;
    //第三方向微信终端发送一个SendAuthReq消息结构
    if ([WXApi isWXAppInstalled]) {
        [WXApi sendReq:req];
    } else {
        [WXApi sendAuthReq:req viewController:UnityGetGLViewController()
                  delegate:self];
    }
}

- (void)processAuthResult:(SendAuthResp *)resp {
    [self sendAuthResult:resp.errCode
                     err:resp.errStr
                    code:resp.code
                   state:resp.state
                    lang:resp.lang
                 country:resp.country];
}

// 向Unity返回授权结果。
- (void)sendAuthResult: (int) res
                   err: (NSString *) errMsg
                  code: (NSString *) code
                 state: (NSString *) state
                  lang: (NSString *) lang
               country: (NSString *) country {
    NSString *resultStr = [NSString stringWithFormat:
                           @"{\"Result\":%d,\"ErrMsg\":\"%@\",\"Code\":\"%@\",\"State\":\"%@\",\"Lang\":\"%@\",\"Country\":\"%@\"}",
                           res,
                           errMsg != nil ? errMsg : @"",
                           code != nil ? code : @"",
                           state != nil ? state : @"",
                           lang != nil ? lang : @"",
                           country != nil ? country : @""];
    
    UnitySendMessage("WeChatManager", "AuthResult", [resultStr UTF8String]);
}

#pragma mark - 微信支付

// 实际微信支付的逻辑。
- (void)wxPay:(NSString *)order {
    if (![WXApi isWXAppInstalled]) {
        [self sendWxPayResult:WXErrCodeCommon
                          msg:@"没有安装微信"];
        return;
    }
    
    // 解析发过来的json文件。
    NSDictionary *res = [Util parseJson:order];
    
    if (res == nil || [res count] <= 0) {
        // 返回解析数据失败的结果。
        [self sendWxPayResult:WXErrCodeCommon msg: @"解析订单数据出错"];
        return;
    }
    
    NSString *str = [NSString stringWithFormat:@"weixin://app/%@/pay/?nonceStr=%@&package=Sign%%3DWXPay&partnerId=%@&prepayId=%@&timeStamp=%@&sign=%@&signType=SHA1",
                     res[@"appId"],
                     res[@"nonceStr"],
                     res[@"partnerId"],
                     res[@"prepayId"],
                     [NSString stringWithFormat:@"%d",[res[@"timeStamp"] intValue]],
                     res[@"sign"]
                     ];
    
    NSURL *url = [NSURL URLWithString:str];
    if (floor(NSFoundationVersionNumber) > NSFoundationVersionNumber_iOS_9_x_Max) {
        [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
    } else {
        if ([[UIApplication sharedApplication] canOpenURL:url]) {
            [[UIApplication sharedApplication] openURL:url];
        } else {
            [self processWxpayResult:WXErrCodeCommon];
        }
    }
}

// 处理微信支付的结果。
- (void)processWxpayResult: (int) errCode {
    NSString *err = nil;
    if (errCode == WXErrCodeUserCancel) {
        err = @"玩家取消支付";
    } else {
        err = @"支付失败";
    }
    
    [self sendWxPayResult: errCode
                      msg: err];
}

// 向Unity返回支付结果。
- (void)sendWxPayResult:(int) errCode msg: (NSString *) errMsg {
    NSString *resultStr = [NSString stringWithFormat:
                           @"{\"Result\":%d,\"ErrMsg\":\"%@\"}",
                           errCode,
                           errMsg != nil ? errMsg : @""];
    
    UnitySendMessage("PayManager", "WxPayResult", [resultStr UTF8String]);
}

#pragma mark - 微信消息回调

- (void)onReq:(BaseReq *)req {
}

- (void)onResp:(BaseResp *)resp {
    if ([resp isKindOfClass:[SendAuthResp class]]) {
        
        [self processAuthResult:(SendAuthResp *) resp];
    } else if ([resp isKindOfClass:[SendMessageToWXResp class]]) {
        
        // 处理微信分享的结果。
        [self processShareResult:(SendMessageToWXResp *) resp];
    }
}

#pragma mark 微信分享

// 处理微信分享的结果。
- (void)processShareResult:(SendMessageToWXResp *)resp {
    switch (resp.errCode) {
        case WXSuccess:
            [self sendWxShareResult:WXSuccess error:nil];
            break;
            
        case WXErrCodeUserCancel:
            [self sendWxShareResult:WXErrCodeUserCancel error:@"玩家取消分享"];
            break;
            
        default:
            [self sendWxShareResult:WXErrCodeCommon error:@"分享失败"];
            break;
    }
}

// 向Unity发送分享的结果。
- (void)sendWxShareResult:(int)result
                    error:(NSString *)errMsg {
    
    _wexinShareContent = [_wexinShareContent stringByReplacingOccurrencesOfString:@"\"" withString:@"\\\""];
    NSString *resultStr = [NSString stringWithFormat:
                           @"{\"Res\":%d,\"ErrMsg\":\"%@\",\"Content\":\"%@\"}",
                           result,
                           errMsg != nil ? errMsg : @"",
                           _wexinShareContent != nil ? _wexinShareContent : @""];
    
    UnitySendMessage("WeChatManager", "WxShareResult", [resultStr UTF8String]);
}

// 执行实际的微信分享逻辑。
- (void)handleWxShare:(NSString *)url
               imgUrl:(NSString *)imgUrl
              imgPath:(NSString *)imgPath
                title:(NSString *)title
              content:(NSString *)content
             thumbUrl:(NSString *)thumbUrl
            wexinShareContent:(NSString *)wexinShareContent
              toScene:(enum WXScene)scene {
    
    _wexinShareContent = wexinShareContent;
    
    if (![WXApi isWXAppInstalled]) {
        [self sendWxShareResult:WXErrCodeCommon error:@"没有安装微信"];
        return;
    }
    
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.scene = scene;
    
    WXMediaMessage *msg = nil;
    // 根据是否有图片和网址来判断是否是文字还是多媒体消息。
    if ((url != nil && url.length > 0) ||
        (imgUrl != nil && imgUrl.length > 0) ||
        (imgPath != nil && imgPath.length > 0))
        msg = [WXMediaMessage message];
    
    if (msg == nil) {
        // 纯文字消息。
        req.text = content;
        req.bText = YES;
        [WXApi sendReq:req];
        return;
    }
    
    // 非文字消息。
    req.bText = NO;
    req.message = msg;
    
    if (title != nil && title.length > 0) msg.title = title;
    // 如果是多媒体消息的情况下，将content当成描述。
    if (content != nil && content.length > 0) msg.description = content;
    
    if (url != nil && url.length > 0) {
        // 分享网页。
        
        // 获取远程的缩略图。
        UIImage *thumb = nil;
        if (thumbUrl != nil) {
            NSData *thumbData = [NSData dataWithContentsOfURL:[NSURL URLWithString:thumbUrl]];
            if (thumbData != nil) {
                UIImage *thumbImg = [UIImage imageWithData:thumbData];
                thumb = [self createThumbFromImage:thumbImg];
            }
        }
        
        // 如果远程缩略图不存在，则使用系统默认的Icon。
        if (thumb == nil)
            thumb = [UIImage imageNamed:@"AppIcon76x76"];
        
        if (thumb != nil)
            [msg setThumbImage:thumb];
        
        WXWebpageObject *ext = [WXWebpageObject object];
        ext.webpageUrl = url;
        msg.mediaObject = ext;
        
        [WXApi sendReq:req];
        return;
    }
    
    if (imgPath != nil && imgPath.length > 0) {
        // 分享图片。
        NSData *imgData = [NSData dataWithContentsOfFile:imgPath];
        UIImage *img = nil;
        if (imgData != nil)
            img = [UIImage imageWithData:imgData];
        
        // 压缩图片的大小。
        UIImage *scaled = [self createSuitableImage:img];
        
        if (scaled == nil) {
            [self sendWxShareResult:WXErrCodeCommon error:@"分享图片失败"];
            return;
        }
        
        WXImageObject *ext = [WXImageObject object];
        ext.imageData = UIImagePNGRepresentation(scaled);
        msg.mediaObject = ext;
        
        // 创建缩略图。
        UIImage *thumb = [self createThumbFromImage:img];
        if (thumb != nil)
            [msg setThumbImage:thumb];
        
        [WXApi sendReq:req];
        return;
    }
    
    if (imgUrl != nil && imgUrl.length > 0) {
        // 分享图片网址。
        UIImage *img = nil;
        UIImage *scaled = nil;
        UIImage *thumbImg = nil;
        
        // 尝试下载远程图片。
        NSData *data = [NSData dataWithContentsOfURL:[NSURL URLWithString:imgUrl]];
        if (data == nil) {
            [self sendWxShareResult:WXErrCodeCommon error:@"分享图片失败"];
            return;
        }
        
        // 创建要分享的图片和图片的缩略图。
        img = [UIImage imageWithData:data];
        scaled = [self createSuitableImage:img];
        thumbImg = [self createThumbFromImage:img];
        
        if (thumbImg != nil) [msg setThumbImage:thumbImg];
        
        if (scaled == nil) {
            [self sendWxShareResult:WXErrCodeCommon error:@"分享图片失败"];
            return;
        }
        
        WXImageObject *ext = [WXImageObject object];
        ext.imageData = UIImagePNGRepresentation(scaled);
        msg.mediaObject = ext;
        
        [WXApi sendReq:req];
        return;
    }
    
    [self sendWxShareResult:WXErrCodeCommon error:@"分享微信失败"];
}

// 创建缩略图。
- (UIImage *)createThumbFromImage:(UIImage *)image {
    if (image == nil) return nil;
    
    // 缩略图的大小限制为32K。
    float sizeLimit = 32 * 1024.0f;
    
    long oldSize = [self sizeOfImage:image];
    if (oldSize < sizeLimit) {
        return image;
    } else {
        float scale = sqrtf(sizeLimit / oldSize);
        return [self scaleImage:image toScale:scale];
    }
}

/*
 * 将分享的图片压缩到合适的大小。
 * < 100K。
 */
- (UIImage *)createSuitableImage:(UIImage *)image {
    if (image == nil) return nil;
    
    // 分享图片的大小限制为4000K。
    float sizeLimit = 4000 * 1024.0f;
    long size = [self sizeOfImage:image];
    if (size <= sizeLimit)
        return image;
    
    float scale = sqrtf(sizeLimit / size);
    return [self scaleImage:image toScale:scale];
}

// 对图片进行缩放。
- (UIImage *)scaleImage:(UIImage *)image
                toScale:(float)scale {
    if (image == nil) return nil;
    
    UIGraphicsBeginImageContext(CGSizeMake(image.size.width * scale, image.size.height * scale));
    [image drawInRect:CGRectMake(0, 0, image.size.width * scale, image.size.height * scale)];
    UIImage *scaledImg = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return scaledImg;
}

// 计算图片的大小。(byte)
- (long)sizeOfImage:(UIImage *)image {
    if (image == nil) return 0;
    
    size_t bytes = CGImageGetBytesPerRow(image.CGImage) * CGImageGetHeight(image.CGImage);
    return bytes;
}

@end

extern "C" {
    bool isWechatInstalled() {
        return [WXApi isWXAppInstalled];
    }
    
    void wxInit(char *wxAppId) {
        [WXApi registerApp:[Util charToNSString:wxAppId]];
    }
    
    void wxAuth(char *deviceId) {
        [[WeChatManager instance] wxAuth:[Util charToNSString:deviceId]];
    }
    
    void wxPay(char *order) {
        [[WeChatManager instance] wxPay:[Util charToNSString:order]];
    }
    
    void wxShare(
                 char *url,
                 char *imgUrl,
                 char *imgPath,
                 char *title,
                 char *content,
                 char *thumbUrl,
                 char *wexinShareContent,
                 bool isCircle) {
        [[WeChatManager instance] handleWxShare:[Util charToNSString:url]
                                         imgUrl:[Util charToNSString:imgUrl]
                                        imgPath:[Util charToNSString:imgPath]
                                          title:[Util charToNSString:title]
                                        content:[Util charToNSString:content]
                                       thumbUrl:[Util charToNSString:thumbUrl]
                                      wexinShareContent:[Util charToNSString:wexinShareContent]
                                        toScene:isCircle ? WXSceneTimeline : WXSceneSession];
    }
}
