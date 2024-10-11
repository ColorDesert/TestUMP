#import <Foundation/Foundation.h>
#if !TARGET_OS_WATCH
#import <SystemConfiguration/SystemConfiguration.h>
#import <netinet/in.h>
#import <netinet6/in6.h>
#import <arpa/inet.h>
#import <ifaddrs.h>
#import <netdb.h>

typedef NS_ENUM(NSInteger, UnityPluginNetworkReachabilityStatus) {
    UnityPluginNetworkReachabilityStatusUnknown          = -2,
    UnityPluginNetworkReachabilityStatusNotReachable     = -1,
    UnityPluginNetworkReachabilityStatusViaWWAN = 1,
    UnityPluginNetworkReachabilityStatusViaWiFi = 2,
};


typedef void (^UnityPluginNetworkReachabilityStatusBlock)(UnityPluginNetworkReachabilityStatus status);

NS_ASSUME_NONNULL_BEGIN


@interface UnityPluginNetworkReachabilityManager : NSObject 

@property (readonly, nonatomic, assign) UnityPluginNetworkReachabilityStatus networkReachabilityStatus;

+ (instancetype)sharedManager;

+ (instancetype)new NS_UNAVAILABLE;

- (instancetype)init NS_UNAVAILABLE;

- (void)startMonitoring;

- (void)stopMonitoring;

- (void)setReachabilityStatusChangeBlock:(nullable UnityPluginNetworkReachabilityStatusBlock)block;


@end


NS_ASSUME_NONNULL_END

#endif
