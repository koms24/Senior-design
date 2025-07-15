#ifndef TMC_INST_TMC2209Driver_H_
#define TMC_INST_TMC2209Driver_H_

#include <stddef.h>
#include <stdbool.h>
#include <stdint.h>
typedef void (*callbackTakeIntReturnsVoid)(int);
typedef int32_t (*uartInterface)(int16_t, uint8_t*, size_t , size_t);

typedef struct {
	uint16_t numIc;
	uint8_t* addressMap;
} TmcDriverConfig;

// #ifdef __cplusplus
// extern "C" {
// #endif

// #ifdef _WIN32
// #	ifdef MODULE_API_EXPORTS
// #		define MODULE_API __declspec(dllexport)
// #	else
// #		define MODULE_API __declspec(dllimport)
// #	endif
// #else
// #define MODULE_API
// #endif
// MODULE_API void SetErrorCallback(callbackTakeIntReturnsVoid cb);
// MODULE_API void SetNodeAddress(uint8_t nadr);
// MODULE_API void SetUartInterfaceCallback(uartInterface cb);
// #ifdef __cplusplus
// }
// #endif

void InitTmcDriver(int16_t numIc, uint8_t* icAddresses);
void DeinitTmcDriver(void);

void SetErrorCallback(callbackTakeIntReturnsVoid cb);
void SetUartInterfaceCallback(uartInterface cb);

void TmcWriteRegister(int16_t icId, uint8_t address, int32_t value);
int32_t TmcReadRegister(int16_t icId, uint8_t address);


#endif