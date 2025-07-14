#pragma once

#include "include_sd_tf.h"

SDTFAPI(uint64_t) sdtf_TfController_sizeof() {
    return sizeof(SD_TF_Controller);
}

SDTFAPI(int) sdtf_TfController_new(SD_TF_Controller **returnValue, int height, int width, int channels, const char *model_dir) {
    try {
        *returnValue = new SD_TF_Controller(height,width,channels, std::string(model_dir));
        return 1;
    } catch(const std::exception&) {
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_delete(SD_TF_Controller** self) {
    try {
        SD_TF_Controller* s = *self;
        if (s != NULL) {
            delete s;
            *self = NULL;
            return 1;
        }
        return 0;
    }
    catch (const std::exception&) {
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_initInferReturnStruct(SD_TF_Controller *self, int batchSize, _cs_return_type **r_ptr) {
    size_t ds = batchSize * sizeof(_cs_return_type);
    *r_ptr = (_cs_return_type*)malloc(ds);
    memset((void*)*r_ptr, 0, ds);
    (*r_ptr)[batchSize - 1].__last_size = batchSize;
    //return _sdtf_TfController_initInferReturnStruct(batchSize, r_ptr);
    //std::cerr << "DEBUG::sdtf_TfController_initInferReturnStruct::Start" << std::endl;
    //std::cerr << "Ptr  Adr: " << std::hex << r_ptr << std::endl << "Self Adr: " << std::hex << self << std::endl;
    //r_ptr[0].detection_cnts = 10;
    //r_ptr[0].ptr_detection_boxes_mat = (CvMat*)43;
    //return 1;
    try {
        (*r_ptr) = self->NewInferReturnStructArray(batchSize);
        //self->InitInferReturnStructArray(batchSize, *r_ptr);
        return 1;
    } catch(const std::exception&) {
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_freeInferReturnStruct(_cs_return_type *r_ptr, int batchSize) {
    if (r_ptr == NULL)
        return 0;
    if (batchSize <= 0) {
        batchSize = 0;
        while (r_ptr[batchSize++].__last_size != 0);
    }
    for(int i=0; i<batchSize; i++) {
        for(int j=0; j<5; j++) {
            if(r_ptr[i]._mat_ptr_ary[j] != NULL) {
                delete r_ptr[i]._mat_ptr_ary[j];
                r_ptr[i]._mat_ptr_ary[j] = NULL;
            }
        }
    }
    free((void*)r_ptr);
    return 1;
    //try {
    //    self->DeinitInferReturnStructArray(batchSize, r_ptr);
    //    return 1;
    //} catch(const std::exception&) {
    //    return -1;
    //}
}

SDTFAPI(int) sdtf_TfController_deleteInferReturnStructArray(_cs_return_type **r_ptr, int batchSize=0) {
    try {
        SD_TF_Controller::DeleteInferReturnStructArray(batchSize, r_ptr);
        return 1;
    }
    catch (const std::exception&) {
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_runInferOnBatchWithBboxConvert(SD_TF_Controller* self, CvMat* img_in, int32_t height, int32_t width, _cs_return_type** r_ptr) {
    try {
        self->RunInferOnBatchWithBboxConvert(img_in, height, width, r_ptr);
        return 1;
    }
    catch (const std::exception& e) {
        std::cerr << e.what() << std::endl;
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_runInferenceOnImage(SD_TF_Controller *self, CvMat *img_in, _cs_return_type **r_ptr) {
    try {
        self->RunInferenceOnImage(img_in,r_ptr);
        return 1;
    } catch(const std::exception& e) {
        std::cerr << e.what() << std::endl;
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_runAsBatch(SD_TF_Controller* self, CvMat* img_in, _cs_return_type** r_ptr) {
    try {
        self->RunAsBatch(img_in, r_ptr);
        return 1;
    }
    catch (const std::exception& e) {
        std::cerr << e.what() << std::endl;
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_getInferReturnStruct(_cs_return_type* r_ptr, int idx, _cs_return_type* r_dat) {
    try {
        //r_ptr[idx].__next_ptr = r_ptr[idx].__last_size != 0 ? NULL : &r_ptr[idx + 1];
        memcpy(r_dat, r_ptr, sizeof(_cs_return_type));
    }
    catch (const std::exception&) {
        return -1;
    }
    return 1;
}

SDTFAPI(int) sdtf_TfController_loadTestImage(SD_TF_Controller *self, CvMat **img_in, const char *filename) {
    try {
        self->LoadTestImage(img_in,filename);
        return 1;
    } catch(const std::exception&) {
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_freeImage(SD_TF_Controller *self, CvMat **img_in) {
    try {
        self->FreeImage(img_in);
        return 1;
    } catch(const std::exception&) {
        return -1;
    }
}

SDTFAPI(int) sdtf_TfController_debugPrintInferResult(SD_TF_Controller *self, int imgCnt, _cs_return_type *r_ptr) {
    try {
        self->DebugPrintInferResult(imgCnt,r_ptr);
        return 1;
    } catch(const std::exception&) {
        return -1;
    }
}