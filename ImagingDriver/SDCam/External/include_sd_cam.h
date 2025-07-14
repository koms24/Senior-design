#pragma once

//#define ENABLED_CONTRIB
//#undef ENABLED_CONTRIB

#ifndef SDC_EXPORTS
# if (defined _WIN32 || defined WINCE || defined __CYGWIN__)
#   define SDC_EXPORTS __declspec(dllexport)
# elif defined __GNUC__ && __GNUC__ >= 4 && defined(__APPLE__)
#   define SDC_EXPORTS __attribute__ ((visibility ("default")))
# endif
#endif

#ifndef SDC_EXPORTS
# define SDC_EXPORTS
#endif

#ifdef _MSC_VER
// ReSharper disable once IdentifierTypo
#define NOMINMAX
// ReSharper disable once CppInconsistentNaming
#define _CRT_SECURE_NO_WARNINGS
#pragma warning(push)
#pragma warning(disable: 4244)
#pragma warning(disable: 4251)
#pragma warning(disable: 4819)
#pragma warning(disable: 4996)
#pragma warning(disable: 6294)
#include <codeanalysis/warnings.h>
#pragma warning(disable: ALL_CODE_ANALYSIS_WARNINGS)
#endif

#define OPENCV_TRAITS_ENABLE_DEPRECATED

#include "sd_cam_controller.hpp"

#if defined WIN32 || defined _WIN32
#  define SDC_CDECL __cdecl
#  define SDC_STDCALL __stdcall
#else
#  define SDC_CDECL
#  define SDC_STDCALL
#endif

#ifndef SDC_EXTERN_C
#   ifdef __cplusplus
#       define SDC_EXTERN_C extern "C"
#   else
#       define SDC_EXTERN_C
#   endif
#endif

#ifndef SDCAPI
#  define SDCAPI(rettype) SDC_EXTERN_C SDC_EXPORTS rettype SDC_CDECL
#endif