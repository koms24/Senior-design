#pragma once

//#define ENABLED_CONTRIB
//#undef ENABLED_CONTRIB

#ifndef SDTF_EXPORTS
# if (defined _WIN32 || defined WINCE || defined __CYGWIN__)
#   define SDTF_EXPORTS __declspec(dllexport)
# elif defined __GNUC__ && __GNUC__ >= 4 && defined(__APPLE__)
#   define SDTF_EXPORTS __attribute__ ((visibility ("default")))
# endif
#endif

#ifndef SDTF_EXPORTS
# define SDTF_EXPORTS
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

#include "sd_tf_controller.hpp"

#if defined WIN32 || defined _WIN32
#  define SDTF_CDECL __cdecl
#  define SDTF_STDCALL __stdcall
#else
#  define SDTF_CDECL
#  define SDTF_STDCALL
#endif

#ifndef SDTF_EXTERN_C
#   ifdef __cplusplus
#       define SDTF_EXTERN_C extern "C"
#   else
#       define SDTF_EXTERN_C
#   endif
#endif

#ifndef SDTFAPI
#  define SDTFAPI(rettype) SDTF_EXTERN_C SDTF_EXPORTS rettype SDTF_CDECL
#endif