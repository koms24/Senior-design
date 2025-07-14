#include <stdio.h>
#include <math.h>
#include <stdlib.h>

// Function to normalize a matrix
void normalize(double* data, int rows, int cols, double* normalizedData) {
    for (int j = 0; j < cols; j++) {
        double min = data[j], max = data[j];
        for (int i = 1; i < rows; i++) {
            if (data[i * cols + j] < min) min = data[i * cols + j];
            if (data[i * cols + j] > max) max = data[i * cols + j];
        }
        for (int i = 0; i < rows; i++) {
            if (max - min == 0)
                normalizedData[i * cols + j] = 0;
            else
                normalizedData[i * cols + j] = (data[i * cols + j] - min) / (max - min);
        }
    }
}

// Function to calculate Euclidean distance
double euclideanDistance(double* vec1, double* vec2, int length) {
    double sum = 0.0;
    for (int i = 0; i < length; i++) {
        sum += pow(vec1[i] - vec2[i], 2);
    }
    return sqrt(sum);
}

// Function to check if a new sensor reading is an anomaly
void checkAnomaly(double* historicalData, int numRows, int numCols, 
                  double* newReading, int k, double anomalyThreshold) {
    int totalRows = numRows + 1;
    double* allData = (double*)malloc(totalRows * numCols * sizeof(double));
    double* normalizedData = (double*)malloc(totalRows * numCols * sizeof(double));
    
    // Combine historical data and new reading
    for (int i = 0; i < numRows * numCols; i++) {
        allData[i] = historicalData[i];
    }
    for (int j = 0; j < numCols; j++) {
        allData[numRows * numCols + j] = newReading[j];
    }
    
    // Normalize the data
    normalize(allData, totalRows, numCols, normalizedData);
    
    // Separate normalized data
    double* historicalDataNormalized = normalizedData;
    double* newReadingNormalized = &normalizedData[numRows * numCols];
    
    // Calculate distances
    double* distances = (double*)malloc(numRows * sizeof(double));
    for (int i = 0; i < numRows; i++) {
        distances[i] = euclideanDistance(&historicalDataNormalized[i * numCols], newReadingNormalized, numCols);
    }
    
    // Sort distances and find k-nearest neighbors
    for (int i = 0; i < numRows - 1; i++) {
        for (int j = i + 1; j < numRows; j++) {
            if (distances[i] > distances[j]) {
                double temp = distances[i];
                distances[i] = distances[j];
                distances[j] = temp;
            }
        }
    }
    
    // Calculate mean distance of the k-nearest neighbors
    double meanDistance = 0.0;
    for (int i = 0; i < k; i++) {
        meanDistance += distances[i];
    }
    meanDistance /= k;
    
    // Check if mean distance exceeds anomaly threshold
    if (meanDistance > anomalyThreshold) {
        printf("Anomaly detected: New reading differs significantly from historical data.\n");
    } else {
        printf("No anomaly detected: New reading is within normal range.\n");
    }
    
    // Display distances for reference
    printf("Distances to nearest neighbors:\n");
    for (int i = 0; i < k; i++) {
        printf("%f\n", distances[i]);
    }
    printf("Mean Distance: %f\n", meanDistance);
    printf("Anomaly Threshold: %f\n", anomalyThreshold);
    
    // Free allocated memory
    free(allData);
    free(normalizedData);
    free(distances);
}

int main() {
    // Example usage
    double historicalData[6][5] = {
        {1.0, 2.0, 3.0, 4.0, 5.0},
        {2.0, 3.0, 4.0, 5.0, 6.0},
        {3.0, 4.0, 5.0, 6.0, 7.0},
        {4.0, 5.0, 6.0, 7.0, 8.0},
        {5.0, 6.0, 7.0, 8.0, 9.0},
        {6.0, 7.0, 8.0, 9.0, 10.0}
    };
    double newReading[5] = {5.5, 6.5, 7.5, 8.5, 9.5};
    int k = 3;
    double anomalyThreshold = 1.0;
    
    checkAnomaly((double*)historicalData, 6, 5, newReading, k, anomalyThreshold);
    
    return 0;
}
