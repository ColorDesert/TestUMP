#import "UnityPluginNetworkReachabilityManager.h"

typedef void (*UnityPluginNetworkStatusCallback)(long status);

#ifdef __cplusplus
extern "C"{
#endif


long _UnityPluginTools_getNetworkStatus()
{
    return [UnityPluginNetworkReachabilityManager sharedManager].networkReachabilityStatus;
}

void _UnityPluginTools_startNetworkStatusMonitoring(UnityPluginNetworkStatusCallback callback)
{
    [[UnityPluginNetworkReachabilityManager sharedManager] startMonitoring];
    [[UnityPluginNetworkReachabilityManager sharedManager] setReachabilityStatusChangeBlock:^(UnityPluginNetworkReachabilityStatus status){
         callback(status);
    }];
    
}

void _UnityPluginTools_stopNetworkStatusMonitoring()
{
    [[UnityPluginNetworkReachabilityManager sharedManager] stopMonitoring];
}


#ifdef __cplusplus
}
#endif
