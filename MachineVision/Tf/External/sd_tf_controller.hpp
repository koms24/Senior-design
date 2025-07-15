#pragma once

#include <algorithm>
#include <cmath>
#include <stdint.h>
#include <iostream>
#include <fstream>
#include <string>
#include <cstring>
#include <sstream>
#include <chrono>

#include <tensorflow/cc/saved_model/loader.h>
#include <tensorflow/core/platform/env.h>
#include <opencv2/opencv.hpp>
#include <opencv2/dnn.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>

#include "sd_tf_types.h"

using namespace std::placeholders;

template<typename K, typename V>
static std::map<V, K> reverse_map(const std::map<K, V>& m) {
    std::map<V, K> r;
    for (const auto& kv : m)
        r[kv.second] = kv.first;
    return r;
}

std::map<int,int> typeMap_TF2CV = {
    {tensorflow::DT_FLOAT, CV_32F},
    {tensorflow::DT_DOUBLE, CV_64F},
    {tensorflow::DT_INT32, CV_32S},
    {tensorflow::DT_UINT8, CV_8U},
    {tensorflow::DT_INT16, CV_16S},
    {tensorflow::DT_INT8, CV_8S},
    {tensorflow::DT_INT64, CV_BIG_INT(8)},
    {tensorflow::DT_BFLOAT16, CV_16F},
    {tensorflow::DT_UINT16, CV_16U},
    {tensorflow::DT_UINT64, CV_BIG_UINT(8)}
};

class SD_TF_Controller {
    private:
    std::string _model_dir = "./exported_model/";
    typedef struct __tf_node_name {
        std::string default_display_name;
        std::string default_name;
        int model_offset;
        int local_offset;
    } __tf_node_name;
    typedef struct __tf_tensor_map {
        __tf_node_name id;
        std::vector<int> tf_size;
        std::vector<int> cv_size;
        size_t cv_alloc_size;
        size_t cv_elment_alloc_bytes;
        size_t cv_alloc_bytes;
        tensorflow::DataType tf_dtype;
        int cv_type;
    } _tf_tensor_map;
    typedef struct __tf_sig_info {
        __tf_node_name id;
        int inputs_len;
        int outputs_len;
        std::vector<std::pair<std::string, tensorflow::Tensor>> input_templates;
        std::vector<std::string> output_tensor_names;
        std::map<int, _tf_tensor_map*> output_lookup;
        std::map<std::string, _tf_tensor_map*> input_map;
        _tf_tensor_map *inputs;
        _tf_tensor_map *outputs;
    } _tf_sig_info;
    typedef struct __tf_model_info {
        int signatures_len;
        int io_tensors_len;
        _tf_sig_info *signatures;
        _tf_tensor_map *io_tensors;
        std::map<std::string, _tf_sig_info*> sig_name_map;
    } _tf_model_info;
    _tf_model_info _model_meta;
    //typedef struct __tf_t_info {
    //    std::string default_display_name;
    //    std::string default_name;
    //    tensorflow::DataType dtype;
    //    std::vector<int> tf_size;
    //    std::vector<int> cv_size;
    //    int cv_type;
    //    std::vector<int> cv_init_val;
    //} _tf_t_info;
    //_tf_t_info _default_input;
    //std::vector<_tf_t_info> _default_outputs;
    // Load the SavedModel
    tensorflow::SavedModelBundle _bundle;
    tensorflow::SessionOptions _session_options;
    tensorflow::RunOptions _run_options;
    //tensorflow::NodeDef& _init_op_node;
    enum SD_DETECT_IDX {
        D_BOXES = 0,
        D_CLASSES,
        D_MASKS,
        D_SCORES,
        IMG_INFO,
        NUM_D
    };
    std::map<std::string, int> _model_io_name_to_struct_map = {
        {"inputs",0},
        {"detection_boxes",0},
        {"detection_classes",1},
        {"detection_masks",2},
        {"detection_scores",3},
        {"image_info",4},
        {"num_detections",5},
    };

    std::map<int, std::string> _model_io_struct_idx_to_name_map = reverse_map(_model_io_name_to_struct_map);

    public:

    struct _padding {
        union {
            int dims[4];
            struct {
                union {
                    int topBottom[2];
                    struct {
                        int top;
                        int bottom;
                    };
                };
                union {
                    int leftRight[2];
                    struct {
                        int left;
                        int right;
                    };
                };
            };
        };
    };

    struct _imgDims {
        int height;
        int width;
    };
    struct _imgMetaDat {
        int channels;
        struct _imgDims orig;
        struct _imgDims resized;
        struct _imgDims scaled;
        struct _padding padding;
    };
    struct _imgMetaDat _imgDat;
    SD_TF_Controller(int height, int width, int channels, std::string model_dir = "") {
        _model_meta.io_tensors_len = 0;
        _model_meta.signatures_len = 0;
        if (!model_dir.empty())
            _model_dir = model_dir;
        _imgDat.channels = channels;
        _imgDat.orig.height = height;
        _imgDat.orig.width = width;
        _imgDat.resized.height = 256;
        _imgDat.resized.width = 256;
        for(int i=0; i<4; i++) _imgDat.padding.dims[i] = 0;
        tensorflow::Status status = tensorflow::LoadSavedModel(_session_options, _run_options, _model_dir, {"serve"}, &_bundle);
        if (!status.ok()) {
            throw std::runtime_error("Error loading SavedModel: "+status.ToString());
        }
        //InitModelIo();
        InitModelMeta();
        CalcScaleSizePadding();
        //for (int i = 0, il = _model_meta.signatures_len; i < il; i++) {
        //    std::cerr << std::endl << _model_meta.signatures[i].id.default_display_name << std::endl << "Inputs:" << std::endl;
        //    for (int j = 0, jl = _model_meta.signatures[i].inputs_len; j < jl; j++) {
        //        std::cerr << _model_meta.signatures[i].inputs[j].id.default_display_name << " : " << _model_meta.signatures[i].inputs[j].id.default_name << std::endl;
        //        std::cerr << "dtype: " << tensorflow::DataType_Name(_model_meta.signatures[i].inputs[j].tf_dtype) <<"  tf_size: " << pVecInt(_model_meta.signatures[i].inputs[j].tf_size).str();
        //        std::cerr << "cv_size: " << pVecInt(_model_meta.signatures[i].inputs[j].cv_size).str();
        //    }
        //    std::cerr << "Outputs:" << std::endl;
        //    for (int j = 0, jl = _model_meta.signatures[i].outputs_len; j < jl; j++) {
        //        std::cerr << _model_meta.signatures[i].outputs[j].id.default_display_name << " : " << _model_meta.signatures[i].outputs[j].id.default_name << std::endl;
        //        std::cerr << "tf_size: " << pVecInt(_model_meta.signatures[i].outputs[j].tf_size).str();
        //        std::cerr << "cv_size: " << pVecInt(_model_meta.signatures[i].outputs[j].cv_size).str();
        //        //for (int k = 0, kl = _model_meta.signatures[i].outputs[j].cv_size.size(); k < kl; k++) {
        //        //    std::cerr << " " << _model_meta.signatures[i].outputs[j].cv_size[k];
        //        //}
        //        //std::cerr << std::endl;
        //    }
        //}
        _runInferenceOnEmptyTensor();
    }
    ~SD_TF_Controller() {
        DeinitModelIo();
    }

    void DeinitModelIo() {
        if(_model_meta.signatures != NULL)
            delete[] _model_meta.signatures;
        if (_model_meta.io_tensors != NULL)
            delete[] _model_meta.io_tensors;
    }

    void InitModelMeta() {
        const auto& signature_def_map = _bundle.meta_graph_def.signature_def();
        int sig_len = signature_def_map.size();
        _model_meta.signatures = new _tf_sig_info[sig_len];
        _model_meta.signatures_len = sig_len;
        int in_len = 0;
        int out_len = 0;
        for (const auto& pair : signature_def_map) {
            const tensorflow::SignatureDef& signature_def = pair.second;
            in_len += signature_def.inputs().size();
            out_len += signature_def.outputs().size();
        }
        in_len = std::max(in_len, sig_len);
        out_len = std::max(out_len, sig_len);
        _model_meta.io_tensors = new _tf_tensor_map[(in_len + out_len)];

        int sig_idx = 0;
        int io_ten_idx = 0;
        for (const auto& pair : signature_def_map) {
            const tensorflow::SignatureDef& signature_def = pair.second;
            _tf_sig_info *s = _model_meta.signatures;
            //std::cerr << pair.first << std::endl;
            s[sig_idx].id.default_display_name = pair.first;
            s[sig_idx].id.default_name = pair.first;
            s[sig_idx].id.local_offset = -1;
            s[sig_idx].id.model_offset = sig_idx;
            s[sig_idx].inputs_len = signature_def.inputs().size();
            s[sig_idx].outputs_len = signature_def.outputs().size();
            s[sig_idx].inputs = _model_meta.io_tensors + io_ten_idx;
            io_ten_idx += s[sig_idx].inputs_len;
            s[sig_idx].outputs = _model_meta.io_tensors + io_ten_idx;
            io_ten_idx += s[sig_idx].outputs_len;
            _model_meta.sig_name_map[pair.first] = &(s[sig_idx]);

            int t_idx = 0;
            for (const auto& _t : signature_def.inputs()) {
                _tf_tensor_map *t = s[sig_idx].inputs;
                t[t_idx].id.default_display_name = _t.first;
                t[t_idx].id.default_name = _t.second.name();
                t[t_idx].id.model_offset = t_idx;
                t[t_idx].id.local_offset = _model_io_name_to_struct_map[_t.first];
                t[t_idx].tf_dtype = _t.second.dtype();
                t[t_idx].cv_type = typeMap_TF2CV[(int)t[t_idx].tf_dtype];
                const auto& tensor_info = _t.second.tensor_shape();
                int t_dim_size = tensor_info.dim_size();
                std::vector<int> tfs(t_dim_size, 0);
                std::vector<int> cvs(t_dim_size, 0);
                //t[t_idx].tf_size.reserve(t_dim_size);
                //t[t_idx].cv_size.reserve(t_dim_size);
                for (int i = 0; i < t_dim_size; i++) {
                    auto tmp = tensor_info.dim(i).size();
                    tfs[i] = tmp;
                    cvs[i] = tmp;
                    //t[t_idx].tf_size[i] = tmp;
                    //t[t_idx].cv_size[i] = tmp;
                }
                t[t_idx].tf_size = tfs;
                t[t_idx].cv_size = cvs;
                s[sig_idx].input_map[_t.first] = &(t[t_idx]);
                //std::cerr << _t.first << std::endl;
                
                t_idx++;
            }

            t_idx = 0;
            for (const auto& _t : signature_def.outputs()) {
                _tf_tensor_map *t = s[sig_idx].outputs;
                //std::cerr << _t.second.name() << std::endl;
                t[t_idx].id.default_display_name = _t.first;
                t[t_idx].id.default_name = _t.second.name();
                t[t_idx].id.model_offset = t_idx;
                t[t_idx].id.local_offset = _model_io_name_to_struct_map[_t.first];
                t[t_idx].tf_dtype = _t.second.dtype();
                t[t_idx].cv_type = typeMap_TF2CV[(int)t[t_idx].tf_dtype];
                const auto& tensor_info = _t.second.tensor_shape();
                int t_dim_size = tensor_info.dim_size();
                std::vector<int> tfs(t_dim_size, 0);
                std::vector<int> cvs((t_dim_size - 1) <= 1 ? 2 : (t_dim_size - 1), 0);
                //t[t_idx].tf_size.reserve(t_dim_size);
                //t[t_idx].cv_size.reserve((t_dim_size - 1) <= 1 ? 2 : (t_dim_size - 1));
                for (int i = 0; i < t_dim_size; i++) {
                    auto tmp = tensor_info.dim(i).size();
                    tfs[i] = tmp;
                    if (i >= 1) {
                        cvs[i-1] = tmp;
                    }
                }
                if ((t_dim_size - 1) < 1) {
                    cvs[0] = 1;
                }
                if ((t_dim_size - 1) < 2) {
                    cvs[1] = 1;
                }
                t[t_idx].tf_size = tfs;
                t[t_idx].cv_size = cvs;
                s[sig_idx].output_lookup[t[t_idx].id.local_offset] = &(t[t_idx]);
                
                
                t_idx++;
            }

            sig_idx++;
        }
    }

    //void InitModelIo() {
    //    const auto& signature_def_map = _bundle.meta_graph_def.signature_def();
    //    const auto& signature_def = signature_def_map.at("serving_default");

    //    for(const auto& _in : signature_def.inputs()) {
    //        tensorflow::DataType tf_dtype = _in.second.dtype();
    //        int cv_dtype = typeMap_TF2CV[(int)tf_dtype];
    //        //std::cerr << "INPUT::" << _in.first << "::" << _in.second.name() << " : cvt: " << cv_dtype << " :: [ ";
    //        const auto& tensor_info = _in.second.tensor_shape();
    //        int l=tensor_info.dim_size();
    //        std::vector<int> cv_sz(l,0);
    //        for(int i=0;i<l;i++) {
    //            cv_sz[i] = tensor_info.dim(i).size();
    //            //if(i!=0) std::cerr << " x ";
    //            //std::cerr << cv_sz[i];
    //        }
    //        //std::cerr << " ]" << std::endl;
    //        _default_input = {
    //            .default_display_name = _in.first,
    //            .default_name = _in.second.name(),
    //            .dtype = tf_dtype,
    //            .tf_size = cv_sz,
    //            .cv_size = cv_sz,
    //            .cv_type = cv_dtype,
    //            .cv_init_val = std::vector<int>(l, 0)
    //        };
    //    }
    //    int o_cnt = 0;
    //    _default_outputs = std::vector<_tf_t_info>(signature_def.outputs().size());
    //    for(const auto& _out : signature_def.outputs()) {
    //        tensorflow::DataType tf_dtype = _out.second.dtype();
    //        int cv_dtype = typeMap_TF2CV[(int)tf_dtype];
    //        //std::cerr << "INPUT::" << _out.first << "::" << _out.second.name() << " : cvt: " << cv_dtype << " :: [ ";
    //        const auto& tensor_info = _out.second.tensor_shape();
    //        int l=tensor_info.dim_size();
    //        std::vector<int> tf_sz(l,0);
    //        std::vector<int> cv_sz((l-1)<=1?2:(l-1),0);
    //        for(int i=0;i<l;i++) {
    //            tf_sz[i] = tensor_info.dim(i).size();
    //            if(i>=1) {
    //                cv_sz[i-1] = tf_sz[i];
    //            }
    //            //if(i!=0) std::cerr << " x ";
    //            //std::cerr << tf_sz[i];
    //        }
    //        if((l-1)<1) {
    //            cv_sz[0] = 1;
    //        }
    //        if((l-1)<2) {
    //            cv_sz[1] = 1;
    //        }
    //        //std::cerr << " ]" << std::endl;
    //        _default_outputs[_model_io_name_to_struct_map[_out.first]] ={
    //            .default_display_name = _out.first,
    //            .default_name = _out.second.name(),
    //            .dtype = tf_dtype,
    //            .tf_size = tf_sz,
    //            .cv_size = cv_sz,
    //            .cv_type = cv_dtype,
    //            .cv_init_val = std::vector<int>(l, 0)
    //        };
    //        ++o_cnt;
    //    }
    //    // for(int i=0;i<5;i++) {
    //    //     std::cerr << "CV_SIZE:" << i << " : [ ";
    //    //     for(int j=0,l=_default_outputs[i].cv_size.size(); j<l; j++) {
    //    //         if(j!=0) std::cerr << " x ";
    //    //         std::cerr << _default_outputs[i].cv_size[j];
    //    //     }
    //    //     std::cerr << " ]" << std::endl;
    //    // }
    //}

    void CalcScaleSizePadding() {
        float scale = std::min((float)_imgDat.resized.height/(float)_imgDat.orig.height, (float)_imgDat.resized.width/(float)_imgDat.resized.height);
        _imgDat.scaled.height = std::round(_imgDat.orig.height * scale);
        _imgDat.scaled.width = std::round(_imgDat.orig.width * scale);
        int vdif = _imgDat.resized.height - _imgDat.scaled.height;
        int hdif = _imgDat.resized.width - _imgDat.scaled.width;
        for(int i=0;i<4;i++) _imgDat.padding.dims[i] = 0;
        if(_imgDat.scaled.width < _imgDat.resized.width) {
            if(hdif > 0)
                _imgDat.padding.right = hdif;
        } else {
            if(vdif > 0)
                _imgDat.padding.bottom = vdif;
        }
    }

    void PreprocessImageCopyToTensor(cv::Mat *img_in, tensorflow::Tensor *tensor) {
        //std::cerr << std::endl << "DEBUG::PreprocessImageCopyToTensor::Start" << std::endl;
        //std::cerr << "DEBUG::InputMatDims:: " << matDimsToString(img_in).str() << std::endl;
        auto _d = _imgDat.scaled;
        //std::cerr << "DEBUG::cv::Mat::alloc:: " << _d.height << " x " << _d.width << std::endl;
        cv::Mat scaled(_d.height,_d.width,CV_8UC3);
        cv::resize(*img_in, scaled, cv::Size(_d.width, _d.height), 0, 0, cv::INTER_CUBIC);
        _d = _imgDat.resized;
        //std::cerr << "DEBUG::cv::Mat::alloc:: " << _d.height << " x " << _d.width << std::endl;
        cv::Mat img_out(_d.height, _d.width, CV_8UC3);
        auto _p = _imgDat.padding;
        //std::cerr << "DEBUG::cv::copyMakeBorder [" << _p.top << ", " << _p.bottom << ", " << _p.left << ", " << _p.right << "]" << std::endl;
        cv::copyMakeBorder(scaled, img_out, _p.top, _p.bottom, _p.left, _p.right, cv::BORDER_CONSTANT, cv::Scalar(0,0,0));
        cv::cvtColor(img_out, img_out, cv::COLOR_BGR2RGB);
        uint8_t* tensor_ptr = tensor->flat<uint8_t>().data();
        memcpy(tensor_ptr, (uint8_t*)img_out.data, _d.height * _d.width * _imgDat.channels * sizeof(uint8_t));
        //std::cerr << "DEBUG::PreprocessImageCopyToTensor::End" << std::endl;
    }

    void _runInferenceOnEmptyTensor(std::string model_sig = "serving_default") {
        auto sig_meta = _model_meta.sig_name_map[model_sig];
        auto ip = sig_meta->inputs;
        auto op = sig_meta->outputs;
        std::vector<tensorflow::Tensor>  outputs;
        tensorflow::TensorShape tensor_shape({ 1, _imgDat.resized.height, _imgDat.resized.width, _imgDat.channels });
        tensorflow::Tensor input_tensor(tensorflow::DT_UINT8, tensor_shape);
        auto session = _bundle.GetSession();
        tensorflow::Status status = session->Run(
            {{ip[0].id.default_name, input_tensor}},
            //{"StatefulPartitionedCall:0","StatefulPartitionedCall:1","StatefulPartitionedCall:2","StatefulPartitionedCall:3","StatefulPartitionedCall:4","StatefulPartitionedCall:5"},
            { op[0].id.default_name, op[1].id.default_name, op[2].id.default_name, op[3].id.default_name, op[4].id.default_name, op[5].id.default_name },
            {},
            &outputs
        );
        if (!status.ok()) {
            throw std::runtime_error("Inference Failed: "+status.ToString());
        }
        //session->Close();
    }

    void RunInferOnBatchWithBboxConvert(cv::Mat* data, int32_t height, int32_t width, _cs_return_type **r_ptr) {
        std::string sig_name = "serving_convert_bboxes";
        //std::cerr << matDimsToString(data).str() << std::endl;
        _tf_sig_info *sig_meta = _model_meta.sig_name_map[sig_name];
        //auto ip = sig_meta->inputs;
        auto imap = sig_meta->input_map;
        auto op = sig_meta->outputs;
        auto sz_vec = getMatMatrixDimsAsVector64(data);
        //std::cerr << pVecInt64(sz_vec).str() << std::endl;
        if (data->dims < 3) {
            auto reshape_vec = getVectInt64ToInt(sz_vec);
            reshape_vec.insert(reshape_vec.begin(), 1);
            sz_vec.insert(sz_vec.begin(), 1);
            //std::cerr << pVecInt(reshape_vec).str() << std::endl;
            cv::Mat *reshaped_mat = new cv::Mat(reshape_vec, data->type(), data->ptr<uint8_t>(0));
            //std::cerr << matDimsToString(reshaped_mat).str() << std::endl;
            //reshaped_mat = data->reshape(0, reshape_vec);
            data = reshaped_mat;
        }
        int batchSize = sz_vec[0];
        //std::cerr << matDimsToString(data).str() << std::endl;

        tensorflow::Status status;
        std::vector<tensorflow::Tensor>  init_outputs;
        //status = _bundle.session->Run({}, {}, { "NoOp" }, &init_outputs);

        //if (!status.ok()) {
        //    throw std::runtime_error("Session Init Failed: " + status.ToString());
        //}
        tensorflow::TensorShape dim_shape({ 2 });
        tensorflow::Tensor dim_in_tensor(tensorflow::DT_INT32, dim_shape);
        int32_t* in_dims = dim_in_tensor.flat<int32_t>().data();
        in_dims[0] = height;
        in_dims[1] = width;


        tensorflow::TensorShape in_shape(absl::MakeSpan(sz_vec));
        tensorflow::Tensor in_tensor(tensorflow::DT_UINT8, in_shape);
        memcpy(in_tensor.flat<uint8_t>().data(), (uint8_t*)(data->data), data->elemSize() * data->total());
        std::vector<tensorflow::Tensor>  outputs;
        status = _bundle.session->Run(
            { {imap["batchInput"]->id.default_name, in_tensor}, {imap["batchImageDims"]->id.default_name, dim_in_tensor} },
            //{"StatefulPartitionedCall:0","StatefulPartitionedCall:1","StatefulPartitionedCall:2","StatefulPartitionedCall:3","StatefulPartitionedCall:4","StatefulPartitionedCall:5"},
            { op[0].id.default_name, op[1].id.default_name, op[2].id.default_name, op[3].id.default_name, op[4].id.default_name, op[5].id.default_name },
            {},
            &outputs
        );
        if (!status.ok()) {
            throw std::runtime_error("Inference Failed: " + status.ToString());
        }
        //std::cerr << "Ran Inference Successfully : BatchSize=" << batchSize << std::endl;
        _cs_return_type* result = NewInferReturnStructArray(batchSize);

        CopyOutputsTensorToResultStructArray(outputs, result, batchSize, sig_name);
        DebugPrintInferResult(batchSize, result, sig_name);
        *r_ptr = result;
    }

    //void RunInferenceOnImage(cv::Mat *img_in, _cs_return_type **r_ptr) {
    //    auto sig_meta = _model_meta.sig_name_map["serving_default"];
    //    auto ip = sig_meta->inputs;
    //    auto op = sig_meta->outputs;
    //    _cs_return_type* result = NewInferReturnStructArray(1);
    //    std::vector<tensorflow::Tensor>  outputs;
    //    tensorflow::TensorShape tensor_shape({ 1, _imgDat.resized.height, _imgDat.resized.width, _imgDat.channels });
    //    tensorflow::Tensor input_tensor(tensorflow::DT_UINT8, tensor_shape);
    //    try {
    //        PreprocessImageCopyToTensor(img_in, &input_tensor);
    //    } catch(const std::exception& e) {
    //        std::cerr << "Exception caught: " << e.what() << std::endl;
    //    }
    //    tensorflow::Status status = _bundle.session->Run(
    //        { {ip[0].id.default_name, input_tensor} },
    //        { op[0].id.default_name, op[1].id.default_name, op[2].id.default_name, op[3].id.default_name, op[4].id.default_name, op[5].id.default_name },
    //        {},
    //        &outputs
    //    );
    //    if (!status.ok()) {
    //        throw std::runtime_error("Inference Failed: "+status.ToString());
    //    }
    //    //std::cerr << "Ran Inference Successfully" << std::endl;
    //    CopyOutputsTensorToResultStructArray(outputs, result, 1, "serving_default");
    //    *r_ptr = result;
    //}

    void RunAsBatch(cv::Mat* img_in, _cs_return_type** r_ptr) {
        //std::cerr << matDimsToString(img_in).str() << std::endl;
        std::string sig_name = "serving_default";
        auto sig_meta = _model_meta.sig_name_map[sig_name];
        auto ip = sig_meta->inputs;
        auto op = sig_meta->outputs;
        int batchSize = img_in->size[0];
        int height = img_in->size[1];
        int width = img_in->size[2];
        //std::cerr << "Batch Size: " << batchSize << std::endl;
        _cs_return_type* result = NewInferReturnStructArray(batchSize);
        tensorflow::TensorShape tensor_shape({ 1, 256, 256, 3 });
        tensorflow::Tensor input_tensor(tensorflow::DT_UINT8, tensor_shape);
        std::vector<std::vector<tensorflow::Tensor>> outputs_map;
        outputs_map.resize(batchSize);
        cv::Mat mbuf(256, 256, CV_8UC3);
        mbuf.setTo(cv::Scalar(0, 0, 0));
        for (int i = 0; i < batchSize; i++) {
            //std::cerr << "Index: " << i << std::endl;
            cv::Mat row = img_in->row(i).reshape(0, {height, width});
            //std::cerr << matDimsToString(&row).str() << std::endl;
            if (row.size[0] != 256 || row.size[1] != 256) {
                cv::resize(row, mbuf, cv::Size(256, 256), 0, 0, cv::INTER_CUBIC);
                cv::cvtColor(mbuf, mbuf, cv::COLOR_BGR2RGB);
            } else {
                cv::cvtColor(row, mbuf, cv::COLOR_BGR2RGB);
            }
            
            uint8_t* tensor_ptr = input_tensor.flat<uint8_t>().data();
            memcpy(tensor_ptr, (uint8_t*)mbuf.data, 256 * 256 * 3 * sizeof(uint8_t));
            //std::vector<tensorflow::Tensor>  outputs;
            tensorflow::Status status = _bundle.session->Run(
                { {ip[0].id.default_name, input_tensor} },
                { op[0].id.default_name, op[1].id.default_name, op[2].id.default_name, op[3].id.default_name, op[4].id.default_name, op[5].id.default_name },
                {},
                &(outputs_map[i])
            );
            if (!status.ok()) {
                throw std::runtime_error("Inference Failed: " + status.ToString());
            }
            CopyOutputsTensorToResultStructArray(outputs_map[i], result, 1, sig_name, i);
        }
        *r_ptr = result;
    }

    void RunInferenceOnImage(cv::Mat* img_in, _cs_return_type** r_ptr) {
        auto sig_meta = _model_meta.sig_name_map["serving_default"];
        auto ip = sig_meta->inputs;
        auto op = sig_meta->outputs;
        _cs_return_type* result = NewInferReturnStructArray(1);
        std::vector<tensorflow::Tensor>  outputs;
        tensorflow::TensorShape tensor_shape({ 1, _imgDat.resized.height, _imgDat.resized.width, _imgDat.channels });
        tensorflow::Tensor input_tensor(tensorflow::DT_UINT8, tensor_shape);
        try {
            PreprocessImageCopyToTensor(img_in, &input_tensor);
        }
        catch (const std::exception& e) {
            std::cerr << "Exception caught: " << e.what() << std::endl;
        }
        tensorflow::Status status = _bundle.session->Run(
            { {ip[0].id.default_name, input_tensor} },
            { op[0].id.default_name, op[1].id.default_name, op[2].id.default_name, op[3].id.default_name, op[4].id.default_name, op[5].id.default_name },
            {},
            &outputs
        );
        if (!status.ok()) {
            throw std::runtime_error("Inference Failed: " + status.ToString());
        }
        //std::cerr << "Ran Inference Successfully" << std::endl;
        CopyOutputsTensorToResultStructArray(outputs, result, 1, "serving_default");
        *r_ptr = result;
    }

    void CopyOutputsTensorToResultStructArray(std::vector<tensorflow::Tensor>& outputs, _cs_return_type *r_ptr, int batchSize, std::string model_sig, int outputIndexOffset = 0) {
        //std::cerr << "Output Size: " << outputs.size() << std::endl;
        auto sig_meta = _model_meta.sig_name_map[model_sig];
        auto op = sig_meta->outputs;
        for (int i = outputIndexOffset, c = 0; c < batchSize; i++,c++) {
            //std::cerr << outputs[D_BOXES].DebugString() << std::endl;
            //std::cerr << outputs[D_CLASSES].DebugString() << std::endl;
            //std::cerr << outputs[D_MASKS].DebugString() << std::endl;
            //std::cerr << outputs[D_SCORES].DebugString() << std::endl;
            //std::cerr << outputs[IMG_INFO].DebugString() << std::endl;
            for (int j = 0; j <= 5; j++) {
                cv::Mat* mat_ptr;
                float* fTensor_ptr;
                int32_t* iTensor_ptr;
                //std::cerr << "Back Map" << op[j].id.default_display_name << " : ";
                try {
                    switch (op[j].id.local_offset) {
                        case (int)D_BOXES:
                            //std::cerr << "D_BOXES  " << outputs[j].DebugString() << std::endl;
                            mat_ptr = r_ptr[i].ptr_detection_boxes_mat;
                            fTensor_ptr = outputs[j].SubSlice(0).flat<float>().data();
                            memcpy((float*)(mat_ptr->data), fTensor_ptr, 100 * 4 * sizeof(float));
                            break;
                        case (int)D_CLASSES:
                            //std::cerr << "D_CLASSES  " << outputs[j].DebugString() << std::endl;
                            mat_ptr = r_ptr[i].ptr_detection_classes_mat;
                            iTensor_ptr = outputs[j].SubSlice(0).flat<int32_t>().data();
                            memcpy((int32_t*)(mat_ptr->data), iTensor_ptr, 100 * sizeof(int32_t));
                            break;
                        case (int)D_MASKS:
                            //std::cerr << "D_MASKS  " << outputs[j].DebugString() << std::endl;
                            mat_ptr = r_ptr[i].ptr_detection_masks_mat;
                            fTensor_ptr = outputs[j].SubSlice(0).flat<float>().data();
                            memcpy((float*)(mat_ptr->data), fTensor_ptr, 100 * 28 * 28 * sizeof(float));
                            break;
                        case (int)D_SCORES:
                            //std::cerr << "D_SCORES  " << outputs[j].DebugString() << std::endl;
                            mat_ptr = r_ptr[i].ptr_detection_scores_mat;
                            fTensor_ptr = outputs[j].SubSlice(0).flat<float>().data();
                            memcpy((float*)(mat_ptr->data), fTensor_ptr, 100 * 1 * sizeof(float));
                            break;
                        case (int)IMG_INFO:
                            //std::cerr << "IMG_INFO  " << outputs[j].DebugString() << std::endl;
                            mat_ptr = r_ptr[i].ptr_image_info_mat;
                            fTensor_ptr = outputs[j].SubSlice(0).flat<float>().data();
                            memcpy((float*)(mat_ptr->data), fTensor_ptr, 4 * 2 * sizeof(float));
                            break;
                        default:
                            //std::cerr << "DETECTION CNT  " << outputs[j].DebugString() << std::endl;
                            r_ptr[i].detection_cnts = outputs[j].SubSlice(0).scalar<int32_t>()();
                            break;
                        }
                } catch(const std::exception& e) {
                    std::cerr << "Error: Copy Back Failed: Field = " << _model_io_struct_idx_to_name_map[op[j].id.local_offset] << std::endl;
                    std::cerr << "Error Msg: " << e.what() << std::endl;
                }
                //std::cerr << "Field Copy Completed: " << j << std::endl;
            }
        }
    }

    void FreeImage(cv::Mat **img) {
        cv::Mat *i = *img;
        if(i!=NULL) {
            delete i;
            *img = NULL;
        }
    }

    void LoadTestImage(cv::Mat **img, const char *filename) {
        cv::Mat i = cv::imread(filename, cv::IMREAD_COLOR);
        *img = new cv::Mat(i);
        i.copyTo(**img);
    }

    _cs_return_type* NewInferReturnStructArray(int batchSize, std::string model_sig = "serving_default") {
        _cs_return_type *r_ptr = new _cs_return_type[batchSize];
        std::memset(r_ptr, 0, sizeof(_cs_return_type) * batchSize);
        //std::cerr << "alloc top: " << r_ptr << std::endl;
        //std::cerr << "alloc size: " << (sizeof(_cs_return_type) * batchSize) << std::endl;
        r_ptr[batchSize - 1].__last_size = batchSize;
        //std::cerr << "set size on last" << std::endl;
        InitInferReturnStructArray(batchSize, r_ptr, model_sig);
        //std::cerr << "return init complete" << std::endl;
        return r_ptr;
    }

    //void CreateInferReturnStructArray(int batchSize, _cs_return_type** r_ptr, std::string model_sig = "serving_default") {
    //    *r_ptr = new _cs_return_type[batchSize];
    //    (*r_ptr)[batchSize - 1].__last_size = batchSize;
    //    InitInferReturnStructArray(batchSize, (*r_ptr), model_sig);
    //}

    static void DeleteInferReturnStructArray(int batchSize, _cs_return_type** r_ptr) {
        _cs_return_type* p = *r_ptr;
        DeinitInferReturnStructArray(batchSize, p);
        delete[] (*r_ptr);
        *r_ptr = (_cs_return_type*)NULL;
    }

    void InitInferReturnStructArray(int batchSize, _cs_return_type *r_ptr, std::string model_sig = "serving_default") {
        //std::cerr << "Base Ptr: " << r_ptr << std::endl;
        //std::cerr << "Derefed Base Ptr: " << &(r_ptr[0]) << std::endl;
        //std::cerr << "Ary Ptr: " << r_ptr[0]._mat_ptr_ary << std::endl;
        auto sig_meta = _model_meta.sig_name_map[model_sig];
        //std::cerr << "Debug" << std::endl;
        auto out_metas = sig_meta->outputs;
        //std::cerr << "Debug" << std::endl;
        for(int i=0,il=batchSize-1; i<batchSize; i++) {
            //std::cerr << "Debug" << std::endl;
            for(int j=0,jl=sig_meta->outputs_len;j<jl;j++) {
                //std::cerr << "Debug:" << j << std::endl;
                int a_offset = out_metas[j].id.local_offset;
                //std::cerr << "Debug" << a_offset << std::endl;
                if (a_offset >= 5)
                    continue;
            //for(int j=0; j<5; j++) {
                //std::cerr << "Debug : i=" << i << std::endl;
                if(r_ptr[i]._mat_ptr_ary[a_offset] != NULL)
                    delete r_ptr[i]._mat_ptr_ary[a_offset];
                //std::cerr << "Debug:" << a_offset << std::endl;
                try {
                    r_ptr[i]._mat_ptr_ary[a_offset] = new cv::Mat(
                        out_metas[j].cv_size,
                        out_metas[j].cv_type
                        //_default_outputs[j].cv_size,
                        //_default_outputs[j].cv_type
                    );
                    r_ptr[i]._mat_ptr_ary[a_offset]->setTo(0);
                } catch(const cv::Exception& e) {
                    std::cerr << "Error: " << e.what() << std::endl;
                }
            }
            r_ptr[i].detection_cnts = 0;
            if (i < il) {
                r_ptr[i].__next_ptr = &(r_ptr[i + 1]);
            }
            // try {
            //     r_ptr[i].ptr_detection_boxes_mat = new cv::Mat(100, 4, CV_32FC1);
            //     r_ptr[i].ptr_detection_classes_mat = new cv::Mat(100, 1, CV_32SC1);
            //     r_ptr[i].ptr_detection_masks_mat = new cv::Mat({100, 28, 28}, CV_32FC1);
            //     r_ptr[i].ptr_detection_scores_mat = new cv::Mat(100, 1, CV_32FC1);
            //     r_ptr[i].ptr_image_info_mat = new cv::Mat(4, 2, CV_32FC1);
            //     r_ptr[i].detection_cnts = 0;
            // } catch(const cv::Exception& e) {
            //     std::cerr << "Error: " << e.what() << std::endl;
            // }
        }
    }

    static void DeinitInferReturnStructArray(int batchSize, _cs_return_type *r_ptr) {
        for(int i=0; i<batchSize; i++) {
            for(int j=0; j<5; j++) {
                if(r_ptr[i]._mat_ptr_ary[j] != NULL) {
                    delete r_ptr[i]._mat_ptr_ary[j];
                    r_ptr[i]._mat_ptr_ary[j] = NULL;
                }
            }
            r_ptr[i].detection_cnts = 0;
        }
    }

    void DebugPrintInferResult(int imgCnt, _cs_return_type *r_ptr, std::string model_sig = "serving_default") {
        if(imgCnt<=0 || r_ptr==NULL)
            return;
        auto sig_meta = _model_meta.sig_name_map[model_sig];
        for(int i=0;i<imgCnt;i++) {
            std::cerr << "Image Index: " << i << std::endl;
            std::cerr << "    Dection Cnt: " << r_ptr[i].detection_cnts << std::endl;
            for(int j=0;j<5;j++)
                std::cerr << "    " << sig_meta->output_lookup[j]->id.default_display_name << ": " << matDimsToString(r_ptr[i]._mat_ptr_ary[j]).str() << std::endl;
            auto m = r_ptr[i]._mat_ptr_ary[IMG_INFO];
            for(int j=0;j<4;j++) {
                std::cerr << "        " << m->at<float>(j, 0) << "    " << m->at<float>(j, 1) << std::endl;
            }
        }
    }

    std::ostringstream matDimsToString(cv::Mat *img) {
        std::ostringstream oss;
        int l = img->dims;
        if(l--<=0)
            return oss;
        auto imgSize = img->size;
        oss << "[";
        for(int i=0; i<l; i++) {
            if(i!=0)
                oss << " x";
            oss << " " << imgSize[i];
        }
        if(l>0)
            oss << " x";
        oss << " " << imgSize[l] << " ]";
        return oss;
    }

    std::ostringstream toStrResultAt(_cs_return_type *r_ptr, int imgIdx, int detectionIdx, std::string model_sig = "serving_default") {
        auto sig_meta = _model_meta.sig_name_map[model_sig];
        //auto out_metas = sig_meta->outputs;
        std::ostringstream oss;
        oss.precision(5);
        std::string lPad = "  ";
        std::string mPad = " ";
        oss << "Image Index: " << imgIdx << mPad << "Dectection Index: " << detectionIdx << std::endl;
        int dim2[] = {4,1,28,1};
        int dim3[] = {1,1,28,1};
        for(int i=0;i<4;i++) {
            oss << sig_meta->output_lookup[i]->id.default_display_name << ":" << std::endl;
            int l = dim2[i];
            if(l>1)
                goto toStrResultAt_2D;
            if(i==1)
                oss << mPad << r_ptr[imgIdx]._mat_ptr_ary[i]->at<int32_t>(detectionIdx,0) << std::endl;
            else
                oss << mPad << r_ptr[imgIdx]._mat_ptr_ary[i]->at<float>(detectionIdx,0) << std::endl;
            continue;
toStrResultAt_2D:
            int j = 0;
            int l2 = dim3[i];
            if(l2>1)
                goto toStrResultAt_3D;
            do {
                oss << mPad << r_ptr[imgIdx]._mat_ptr_ary[i]->at<float>(detectionIdx,j++);
            } while(j<l);
            oss << std::endl;
            continue;
toStrResultAt_3D:
            oss.precision(1);
            oss << std::scientific;
            int k = 0;
            do {
                k = 0;
                do {
                    oss << mPad << r_ptr[imgIdx]._mat_ptr_ary[i]->at<float>(detectionIdx,j,k++);
                } while(k<l2);
                ++j;
                oss << std::endl;
            } while(j<l);
            oss << std::fixed;
            oss.precision(5);
        }
        return oss;
    }

    std::ostringstream pVecInt64(std::vector<int64_t> v) {
        std::ostringstream out;
        for (int i = 0, l = v.size(); i < l; i++)
            out << " " << v[i];
        out << std::endl;
        return out;
    }

    std::ostringstream pVecInt(std::vector<int> v) {
        std::ostringstream out;
        for (int i = 0, l = v.size(); i < l; i++)
            out << " " << v[i];
        out << std::endl;
        return out;
    }

    static std::vector<int64_t> getMatMatrixDimsAsVector64(cv::Mat *mat) {
        const cv::MatSize& size = mat->size;
        int len = size.dims();
        int mat_dim_len = len;
        int channels = mat->channels();
        if (len == mat->dims) {
            if (channels > 1)
                mat_dim_len++;
        }
        std::vector<int64_t> vec(mat_dim_len, 0);
        const int* start_p = size.p;
        const int* end_p = start_p + len;

        std::transform(
            std::vector<int>::const_iterator(size.p),
            std::vector<int>::const_iterator(end_p),
            vec.begin(),
            [](int x) -> int64_t { return static_cast<int64_t>(x); }
        );
        if (mat_dim_len > len)
            vec[mat_dim_len - 1] = channels;

        return vec;
    }

    static std::vector<int> getVectInt64ToInt(std::vector<int64_t> v) {
        std::vector<int> vec(v.size(), 0);

        std::transform(
            v.begin(),
            v.end(),
            vec.begin(),
            [](int64_t x) -> int { return static_cast<int>(x); }
        );

        return vec;
    }
};
