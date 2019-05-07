#import <Foundation/Foundation.h>


@interface Util
+ (NSString *)charToNSString:(const char *)value;

+ (NSDictionary *)parseJson:(NSString *)content;
@end
