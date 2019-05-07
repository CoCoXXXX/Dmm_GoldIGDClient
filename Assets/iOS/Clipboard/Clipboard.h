//
//  Clipboard.h
//  Unity-iPhone
//
//  Created by dmm on 2017/10/30.
//

#import <Foundation/Foundation.h>

@interface Clipboard : NSObject
extern "C"
{
    void _copyTextToClipboard(const char *textList);
}
@end
