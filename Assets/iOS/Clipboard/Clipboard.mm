//
//  Clipboard.m
//  Unity-iPhone
//
//  Created by dmm on 2017/10/30.
//

#import "Clipboard.h"

@implementation Clipboard
//将文本复制到IOS剪贴板
- (void)copyTextToClipboard : (NSString*)text
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = text;
}
@end

extern "C" {
    static Clipboard *iosClipboard;
    
    void copyTextToClipboard(const char *textList)
    {
        NSString *text = [NSString stringWithUTF8String: textList] ;
        
        if(iosClipboard == NULL)
        {
            iosClipboard = [[Clipboard alloc] init];
        }
        
        [iosClipboard copyTextToClipboard: text];
    }
    
}
