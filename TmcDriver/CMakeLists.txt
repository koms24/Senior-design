cmake_minimum_required(VERSION 3.10)

project(tmcapi VERSION 1.0.1 DESCRIPTION "tmc-api")

set(CMAKE_BUILD_TYPE "Debug")

add_library(tmcapi SHARED
    tmc/helpers/Types.h
    tmc/helpers/Macros.h
    tmc/helpers/RegisterAccess.h
    tmc/helpers/Bits.h
    tmc/helpers/Constants.h
    tmc/helpers/Config.h
    tmc/helpers/CRC.c tmc/helpers/CRC.h
    tmc/helpers/API_Header.h
    tmc/helpers/Functions.c tmc/helpers/Functions.h
    tmc/ic/TMC2209/TMC2209_HW_Abstraction.h
    tmc/ic/TMC2209/TMC2209.c tmc/ic/TMC2209/TMC2209.h
    TMC2209Driver.c TMC2209Driver.h
)

include(GenerateExportHeader)
generate_export_header(tmcapi)

