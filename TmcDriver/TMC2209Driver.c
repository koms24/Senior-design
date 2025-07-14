#include "TMC2209Driver.h"
#include "tmc/helpers/Macros.h"
#include "tmc/ic/TMC2209/TMC2209.h"

#include <stdlib.h>
#include <stdio.h>

TmcDriverConfig config = {
    .numIc = 0,
    .addressMap = NULL
};

uint8_t nodeAddress = 0;
callbackTakeIntReturnsVoid errorCallback = NULL;
uartInterface uartInterfaceCallback = NULL;

bool tmc2209_readWriteUART(uint16_t icID, uint8_t *data, size_t writeLength, size_t readLength)
{
    UNUSED(icID);
    int32_t status = (*uartInterfaceCallback)(0, data, writeLength, readLength);
    if(status == -1)
        return false;
    if(readLength>0) {
        for(int i=0; i < readLength; i++)
            printf("%u ", data[i]);
        printf("\n");
    }
    return true;
}

uint8_t tmc2209_getNodeAddress(uint16_t icId)
{
    //UNUSED(icID);
    return config.addressMap[icId];
}

void SetErrorCallback(callbackTakeIntReturnsVoid cb) {
    errorCallback = cb;
}

void SetUartInterfaceCallback(uartInterface cb) {
    uartInterfaceCallback = cb;
}

void TmcWriteRegister(int16_t icId, uint8_t address, int32_t value) {
    tmc2209_writeRegister(icId, address, value);
}
int32_t TmcReadRegister(int16_t icId, uint8_t address) {
    int32_t v = tmc2209_readRegister(icId, address);
    printf("Read Val: %i", v);
    return v;
}

void InitTmcDriver(int16_t numIc, uint8_t* icAddresses) {
    uint16_t _numIc = (uint16_t)numIc;
    if (_numIc <= 0)
        return;
    if (config.addressMap == NULL)
        free(config.addressMap);
    config.numIc = _numIc;
    config.addressMap = (uint8_t*)malloc(_numIc * sizeof(uint8_t));
    for (int i = 0; i < _numIc; i++)
        config.addressMap[i] = icAddresses[i];
    return;
}

void DeinitTmcDriver(void) {
    if (config.addressMap != NULL)
        free(config.addressMap);
    config.numIc = 0;
}
