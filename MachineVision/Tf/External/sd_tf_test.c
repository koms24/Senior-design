#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include "sd_tf_wrapper.h"

int main(void) {
    int aLen = 1;
    _cs_return_type* rdat = NULL;

    void *obj_ptr = NULL;
    int r = sdtf_TfController_new((SD_TF_Controller**)&obj_ptr, 530, 500, 3, "./infer_sv_model/");
    //r &= sdtf_TfController_initInferReturnStruct((SD_TF_Controller*)obj_ptr, aLen, &rdat);



    CvMat *img_ptr = NULL;
    r &= sdtf_TfController_loadTestImage((SD_TF_Controller*)obj_ptr, &img_ptr, "/mnt/f/datasets/CVPPP2017_LSC_training/CVPPP2017_LSC_training/training/A1/plant130_rgb.png");
    //r &= sdtf_TfController_runInferOnBatchWithBboxConvert((SD_TF_Controller*)obj_ptr, img_ptr, &rdat);
    r &= sdtf_TfController_runInferenceOnImage((SD_TF_Controller*)obj_ptr, img_ptr, &rdat);
    r &= sdtf_TfController_debugPrintInferResult((SD_TF_Controller*)obj_ptr, aLen, rdat);
    r &= sdtf_TfController_freeImage((SD_TF_Controller*)obj_ptr, &img_ptr);

    //r &= sdtf_TfController_deleteInferReturnStructArray(&rdat);
    //r &= sdtf_TfController_freeInferReturnStruct(rdat, aLen);
    r &= sdtf_TfController_delete((SD_TF_Controller**)&obj_ptr);


    printf("Exit Status: %d\n", r);
    return r;
}