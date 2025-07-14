#pragma once

#include <stdint.h>
#include <stddef.h>
#include <opencv2/opencv.hpp>

typedef cv::Mat CvMat;

typedef struct __cs_maskrcnn_inference_return_struct {
    union {
        struct {
            CvMat* ptr_detection_boxes_mat;
            CvMat* ptr_detection_classes_mat;
            CvMat* ptr_detection_masks_mat;
            CvMat* ptr_detection_scores_mat;
            CvMat* ptr_image_info_mat;
        } __attribute__((packed));
        CvMat* _mat_ptr_ary[5];
        //void* _void_ptr_ary[5];
    } __attribute__((packed));
    int32_t __reserved_1;
    int32_t detection_cnts;
    struct __cs_maskrcnn_inference_return_struct* __next_ptr;
    int32_t __reserved_2;
    int32_t __last_size;
} __attribute__((packed)) _cs_return_type;
//typedef struct __cs_maskrcnn_inference_return_struct _cs_return_type;

// struct __cs_maskrcnn_inference_return_struct_old {
//     union {
//         void* void_ptr_array[6];
//         struct {
//             CvMat* (*ptr_to_ary_detection_boxes_mat)[];
//             CvMat* (*ptr_to_ary_detection_classes_mat)[];
//             CvMat* (*ptr_to_ary_detection_masks_mat)[];
//             CvMat* (*ptr_to_ary_detection_scores_mat)[];
//             CvMat* (*ptr_to_ary_image_info_mat)[];
//             int (*ptr_to_array_of_detection_cnts)[]; 
//         };
//     };
//     void* __padding[2];
// };
// typedef struct __cs_maskrcnn_inference_return_struct_old* _cs_return_type_ptr;