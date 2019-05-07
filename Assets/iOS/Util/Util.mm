#import "Util.h"
#include <sys/socket.h>
#include <netdb.h>
#include <arpa/inet.h>
#include "SAMKeychain.h"
#include "SAMKeychainQuery.h"

@implementation Util

+ (NSString *)charToNSString:(const char *)value {
    if (value != NULL) {
        return [NSString stringWithUTF8String:value];
    } else {
        return nil;
    }
}

+ (NSDictionary *)parseJson:(NSString *)content {
    if (content == nil)
        return nil;
    
    NSError *error = nil;
    NSData *data = [content dataUsingEncoding:NSUTF8StringEncoding];
    
    return [NSJSONSerialization JSONObjectWithData:data
                                           options:NSJSONReadingMutableContainers
                                             error:&error];
}

static NSString* const service = @"ttigd";

+ (NSString *) getContentForKey:(NSString *) key
                    accessGroup:(NSString *) accessGroup {
    return [SAMKeychain passwordForService:service account:key];
}

+ (BOOL) saveContent: (NSString *) content
              forKey: (NSString *) key
         accessGroup: (NSString *) accessGroup {
    return [SAMKeychain setPassword:content forService:service account:key];
}

+ (BOOL) resetKey: (NSString *) key
      accessGroup: (NSString *) accessGroup {
    return [SAMKeychain deletePasswordForService:service account:key];
}

@end

extern "C" {
    char *makeStringCopy(const char *content) {
        if (content == NULL)
            return NULL;
        
        char *res = (char *) malloc(strlen(content) + 1);
        strcpy(res, content);
        return res;
    }
    
    char* getFromKeyChain(char* key, char* accessGroup) {
        NSString* result = [Util getContentForKey:[Util charToNSString:key]
                                      accessGroup:[Util charToNSString:accessGroup]];
        if (result != nil) {
            return makeStringCopy([result UTF8String]);
        } else {
            return NULL;
        }
    }
    
    bool saveToKeyChain(char* key, char* content, char* accessGroup) {
        return [Util saveContent: [Util charToNSString: content]
                          forKey: [Util charToNSString: key]
                     accessGroup: [Util charToNSString: accessGroup]];
    }
    
    bool resetKeyChainContent(char* key, char* accessGroup) {
        return [Util resetKey:[Util charToNSString: key]
                  accessGroup:[Util charToNSString: accessGroup]];
    }
    
    void openUrl(char *url) {
        if (url == NULL) return;
        
        NSURL *u = [NSURL URLWithString:[Util charToNSString:url]];
        if (floor(NSFoundationVersionNumber) > NSFoundationVersionNumber_iOS_9_x_Max) {
            [[UIApplication sharedApplication] openURL:u options:@{} completionHandler:nil];
        } else {
            if ([[UIApplication sharedApplication] canOpenURL:u]) {
                [[UIApplication sharedApplication] openURL:u];
            }
        }
    }
    
#define CopyString(temp) (temp != NULL)? strdup(temp):NULL
    const char *getIpV6(const char *mHost) {
        if (mHost == NULL)
            return NULL;
        struct addrinfo *res0;
        struct addrinfo hints;
        struct addrinfo *res;
        
        memset(&hints, 0, sizeof(hints));
        
        hints.ai_flags = AI_DEFAULT;
        hints.ai_family = PF_UNSPEC;
        hints.ai_socktype = SOCK_STREAM;
        
        int n;
        if ((n = getaddrinfo(mHost, "http", &hints, &res0)) != 0) {
            return NULL;
        }
        
        struct sockaddr_in6 *addr6;
        struct sockaddr_in *addr;
        const char *pszTemp;
        
        for (res = res0; res; res = res->ai_next) {
            char buf[32];
            if (res->ai_family == AF_INET6) {
                addr6 = (struct sockaddr_in6 *) res->ai_addr;
                pszTemp = inet_ntop(AF_INET6, &addr6->sin6_addr, buf, sizeof(buf));
            } else {
                addr = (struct sockaddr_in *) res->ai_addr;
                pszTemp = inet_ntop(AF_INET, &addr->sin_addr, buf, sizeof(buf));
            }
            
            break;
        }
        
        freeaddrinfo(res0);
        return CopyString(pszTemp);
    }
    
}
