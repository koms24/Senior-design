#pragma once

#include "include_sd_cam.h"

SDCAPI(uint64) sdc_SDCamController_sizeof() {
    return sizeof(SDCamController);
}

SDCAPI(int) sdc_SDCamController_new(SDCamController **returnValue) {
    try {
        *returnValue = new SDCamController;
        return 1;
    } catch(const std::exception&) {
        return -1;
    }
}

SDCAPI(int) sdc_SDCamController_getMatStillFromCamera(SDCamController *self, cv::Mat **returnValue) {
    try {
        *returnValue = new cv::Mat(self->getMatStillFromCamera());
        return 1;
    } catch(const std::exception&) {
        return -1;
    }
}

SDCAPI(int) sdc_SDCamController_delete(SDCamController *self) {
    try {
        delete self;
        return 1;
    } catch(const std::exception&) {
        return -1;
    }
}