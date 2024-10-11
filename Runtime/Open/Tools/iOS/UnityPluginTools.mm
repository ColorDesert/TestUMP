//
//  PZSDKBridge.m
//  PZSDKDemo
//
//  Created by 孙泉 on 2019/5/16.
//  Copyright © 2019 puzzle. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#include <stdlib.h>
#import <string.h>
#import <sys/mount.h>
#import <mach/mach.h>

static char * _UnityPluginTools_StrCpy(NSString *str)
{
    if (!str || ![str isKindOfClass:NSString.class]) {
        return NULL;
    }
    
    const char *src = [str cStringUsingEncoding:NSUTF8StringEncoding];
    
    if (src == NULL) {
        return NULL;
    }

    char *dst = (char *)malloc(strlen(src) + 1);
    strcpy(dst, src);
    return dst;
}

#ifdef __cplusplus
extern "C"{
#endif
    
    void _UnityPluginTools_ShowDialog(const char *title, const char *message)
    {
        NSString *t = [NSString stringWithUTF8String:title];
        NSString *m = [NSString stringWithUTF8String:message];
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:t message:m delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alert show];
    }

    long _UnityPluginTools_FreeDiskSize()
    {
        struct statfs buf;
        unsigned long long space = -1;
        if (statfs("/var", &buf) >= 0) {
            space = (unsigned long long)(buf.f_bsize * buf.f_bavail);
        }
        NSString *size = [NSString stringWithFormat:@"%.0f", ((CGFloat)space) / 1000 / 1000];
        return size.longLongValue;
    }

    long _UnityPluginTools_MemoryUsage()
    {
        task_vm_info_data_t vminfo;
        mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
        kern_return_t kernReturn = task_info(mach_task_self(), TASK_VM_INFO, (task_info_t)&vminfo, &count);
        if(kernReturn != KERN_SUCCESS) {
            return 0;
        }
        long usage = vminfo.phys_footprint;
        return usage / 1024.f / 1024.f;
    }

    long _UnityPluginTools_MemoryAvailable()
    {
        vm_statistics_data_t vmstat;
        mach_msg_type_number_t count = HOST_VM_INFO_COUNT;
        kern_return_t kernReturn = host_statistics(mach_host_self(), HOST_VM_INFO, (host_info_t)&vmstat, &count);
        if (kernReturn != KERN_SUCCESS) {
            return 0;
        }
        
//        long total = NSProcessInfo.processInfo.physicalMemory;
//        long wired = vm_page_size *vmstat.wire_count;
//        long active = vm_page_size *vmstat.active_count;
        long inative = vm_page_size *vmstat.inactive_count;
        long free = vm_page_size *vmstat.free_count;
        
        long available = inative + free;
        return available / 1024.f / 1024.f;
    }
    
    void _UnityPluginTools_Copy(const char * text)
    {
        NSString *t = [NSString stringWithUTF8String:text];
        UIPasteboard.generalPasteboard.string = t;
    }

    char * _UnityPluginTools_Paste()
    {
        return _UnityPluginTools_StrCpy(UIPasteboard.generalPasteboard.string);
    }

    bool _UnityPluginTools_IsNotchScreen()
    {
        if (@available(iOS 11.0, *)) {
            return UIApplication.sharedApplication.keyWindow.safeAreaInsets.bottom > 0;
        } else {
            return false;
        }
    }
    
    float _UnityPluginTools_GetNotchHeight()
    {
        if (@available(iOS 11.0, *)) {
                UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
                switch(orientation) {
                    case UIInterfaceOrientationPortrait:
                    case UIInterfaceOrientationUnknown:{
                        return UIApplication.sharedApplication.keyWindow.safeAreaInsets.top;
                        break;
                    }
                    case UIInterfaceOrientationPortraitUpsideDown:{
                        return UIApplication.sharedApplication.keyWindow.safeAreaInsets.bottom;
                        break;
                    }
                    case UIInterfaceOrientationLandscapeLeft:{
                        return UIApplication.sharedApplication.keyWindow.safeAreaInsets.left;
                        break;
                    }
                    case UIInterfaceOrientationLandscapeRight:{
                        return UIApplication.sharedApplication.keyWindow.safeAreaInsets.right;
                        break;
                    }
                }
        } else {
                return 0.0;
        }
    }
    
    void _UnityPluginTools_TryHideStatusBar(bool hidden)
    {
        if (_UnityPluginTools_IsNotchScreen()) {
            [UIApplication.sharedApplication setStatusBarHidden:hidden withAnimation:UIStatusBarAnimationSlide];
        }
    }

    void _UnityPluginTools_SystemShare(const char *content, const void *image, int length)
    {
        NSString *c = [NSString stringWithUTF8String:content];
        NSData *cda = [c dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *d = [NSJSONSerialization JSONObjectWithData:cda options:NSJSONReadingMutableContainers error:nil];
        NSString *t = [d.allKeys containsObject:@"text"] ? d[@"text"] : nil;
        NSString *u = [d.allKeys containsObject:@"url"] ? d[@"url"] : nil;
        NSData *ida = length ? [NSData dataWithBytes:image length:length] : nil;
        UIImage *i = ida ? [UIImage imageWithData:ida] : nil;
        NSMutableArray *ma = NSMutableArray.array;
        if (t) {
            [ma addObject:t];
        }
        if (u) {
            [ma addObject:u];
        }
        if (i) {
            [ma addObject:i];
        }
        
        if (ma.count <= 0) {
            return;
        }
        
        UIActivityViewController *vc = [[UIActivityViewController alloc] initWithActivityItems:ma applicationActivities:nil];
        __weak __typeof(vc) wvc = vc;
        wvc.completionWithItemsHandler = ^(UIActivityType activityType, BOOL completed, NSArray *returnedItems, NSError *activityError) {
            __strong __typeof(vc) svc = wvc;
            [svc dismissViewControllerAnimated:YES completion:nil];
        };
        [UIApplication.sharedApplication.keyWindow.rootViewController presentViewController:vc animated:YES completion:nil];
    }

    void _UnityPluginTools_Restart()
    {
        exit(0);
    }

    float _UnityPluginTools_BatteryLevel()
    {
        UIDevice.currentDevice.batteryMonitoringEnabled = YES;
        return UIDevice.currentDevice.batteryLevel;
    }

    long _UnityPluginTools_SystemUptime()
    {
        NSTimeInterval systemUptime = [[NSProcessInfo processInfo] systemUptime];
        NSString *time = [NSString stringWithFormat:@"%.0f", systemUptime * 1000];
        return time.longLongValue;
    }

#ifdef __cplusplus
}
#endif
