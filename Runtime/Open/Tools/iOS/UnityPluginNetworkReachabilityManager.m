#import "UnityPluginNetworkReachabilityManager.h"
#if !TARGET_OS_WATCH
#import <SystemConfiguration/SystemConfiguration.h>
#import <netinet/in.h>
#import <netinet6/in6.h>
#import <arpa/inet.h>
#import <ifaddrs.h>
#import <netdb.h>


typedef UnityPluginNetworkReachabilityManager * (^UnityPluginNetworkReachabilityStatusCallback)(UnityPluginNetworkReachabilityStatus status);

static UnityPluginNetworkReachabilityStatus UnityPluginNetworkReachabilityStatusForFlags(SCNetworkReachabilityFlags flags) {
    BOOL isReachable = ((flags & kSCNetworkReachabilityFlagsReachable) != 0);
    BOOL needsConnection = ((flags & kSCNetworkReachabilityFlagsConnectionRequired) != 0);
    BOOL canConnectionAutomatically = (((flags & kSCNetworkReachabilityFlagsConnectionOnDemand ) != 0) || ((flags & kSCNetworkReachabilityFlagsConnectionOnTraffic) != 0));
    BOOL canConnectWithoutUserInteraction = (canConnectionAutomatically && (flags & kSCNetworkReachabilityFlagsInterventionRequired) == 0);
    BOOL isNetworkReachable = (isReachable && (!needsConnection || canConnectWithoutUserInteraction));

    UnityPluginNetworkReachabilityStatus status = UnityPluginNetworkReachabilityStatusUnknown;
    if (isNetworkReachable == NO) {
        status = UnityPluginNetworkReachabilityStatusNotReachable;
    }
#if    TARGET_OS_IPHONE
    else if ((flags & kSCNetworkReachabilityFlagsIsWWAN) != 0) {
        status = UnityPluginNetworkReachabilityStatusViaWWAN;
    }
#endif
    else {
        status = UnityPluginNetworkReachabilityStatusViaWiFi;
    }

    return status;
}

static void UnityPluginPostReachabilityStatusChange(SCNetworkReachabilityFlags flags, UnityPluginNetworkReachabilityStatusCallback block) {
    UnityPluginNetworkReachabilityStatus status = UnityPluginNetworkReachabilityStatusForFlags(flags);
    dispatch_async(dispatch_get_main_queue(), ^{
        UnityPluginNetworkReachabilityManager *manager = nil;
        if (block) {
            manager = block(status);
        }
    });
}

static void UnityPluginNetworkReachabilityCallback(SCNetworkReachabilityRef __unused target, SCNetworkReachabilityFlags flags, void *info) {
    UnityPluginPostReachabilityStatusChange(flags, (__bridge UnityPluginNetworkReachabilityStatusCallback)info);
}

static const void * UnityPluginNetworkReachabilityRetainCallback(const void *info) {
    return Block_copy(info);
}

static void UnityPluginNetworkReachabilityReleaseCallback(const void *info) {
    if (info) {
        Block_release(info);
    }
}

@interface UnityPluginNetworkReachabilityManager ()

@property (readonly, nonatomic, assign) SCNetworkReachabilityRef networkReachability;
@property (readwrite, nonatomic, assign) UnityPluginNetworkReachabilityStatus networkReachabilityStatus;

@property (readwrite, nonatomic, copy) UnityPluginNetworkReachabilityStatusBlock networkReachabilityStatusBlock;

@end



@implementation UnityPluginNetworkReachabilityManager

+ (instancetype)sharedManager {
    static UnityPluginNetworkReachabilityManager *_sharedManager = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _sharedManager = [self manager];
    });

    return _sharedManager;
}

+ (instancetype)manager
{
#if (defined(__IPHONE_OS_VERSION_MIN_REQUIRED) && __IPHONE_OS_VERSION_MIN_REQUIRED >= 90000) || (defined(__MAC_OS_X_VERSION_MIN_REQUIRED) && __MAC_OS_X_VERSION_MIN_REQUIRED >= 101100)
    struct sockaddr_in6 address;
    bzero(&address, sizeof(address));
    address.sin6_len = sizeof(address);
    address.sin6_family = AF_INET6;
#else
    struct sockaddr_in address;
    bzero(&address, sizeof(address));
    address.sin_len = sizeof(address);
    address.sin_family = AF_INET;
#endif
    return [self managerForAddress:&address];
}

+ (instancetype)managerForAddress:(const void *)address {
    SCNetworkReachabilityRef reachability = SCNetworkReachabilityCreateWithAddress(kCFAllocatorDefault, (const struct sockaddr *)address);
    UnityPluginNetworkReachabilityManager *manager = [[self alloc] initWithReachability:reachability];

    CFRelease(reachability);
    
    return manager;
}

- (instancetype)initWithReachability:(SCNetworkReachabilityRef)reachability {
    self = [super init];
    if (!self) {
        return nil;
    }

    _networkReachability = CFRetain(reachability);
    self.networkReachabilityStatus = UnityPluginNetworkReachabilityStatusUnknown;

    return self;
}


- (instancetype)init
{
    return nil;
}

- (void)dealloc {
    [self stopMonitoring];
    
    if (_networkReachability != NULL) {
        CFRelease(_networkReachability);
    }
}

#pragma mark -

- (void)startMonitoring {
    [self stopMonitoring];

    if (!self.networkReachability) {
        return;
    }

    __weak __typeof(self)weakSelf = self;
    UnityPluginNetworkReachabilityStatusCallback callback = ^(UnityPluginNetworkReachabilityStatus status) {
        __strong __typeof(weakSelf)strongSelf = weakSelf;

        strongSelf.networkReachabilityStatus = status;
        if (strongSelf.networkReachabilityStatusBlock) {
            strongSelf.networkReachabilityStatusBlock(status);
        }
        
        return strongSelf;
    };

    SCNetworkReachabilityContext context = {0, (__bridge void *)callback, UnityPluginNetworkReachabilityRetainCallback, UnityPluginNetworkReachabilityReleaseCallback, NULL};
    SCNetworkReachabilitySetCallback(self.networkReachability, UnityPluginNetworkReachabilityCallback, &context);
    SCNetworkReachabilityScheduleWithRunLoop(self.networkReachability, CFRunLoopGetMain(), kCFRunLoopCommonModes);

    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_LOW, 0),^{
        SCNetworkReachabilityFlags flags;
        if (SCNetworkReachabilityGetFlags(self.networkReachability, &flags)) {
            UnityPluginPostReachabilityStatusChange(flags, callback);
        }
    });
}

- (void)stopMonitoring {
    if (!self.networkReachability) {
        return;
    }

    SCNetworkReachabilityUnscheduleFromRunLoop(self.networkReachability, CFRunLoopGetMain(), kCFRunLoopCommonModes);
}

#pragma mark -

- (void)setReachabilityStatusChangeBlock:(void (^)(UnityPluginNetworkReachabilityStatus status))block {
    self.networkReachabilityStatusBlock = block;
}


@end


#endif
