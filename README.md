# Plant Monitoring System

A plant-monitoring system using live camera feed and TensorFlow machine vision on a local host. Designed for users lacking time or resources for plant care.

## 🚀 Project Overview

This system combines multiple technologies to create an intelligent plant monitoring solution:

- **Live Camera Feed**: Real-time video streaming with HLS support
- **Machine Vision**: TensorFlow-based plant health analysis using OpenCV
- **Motor Control**: TMC2209 stepper motor drivers for automated plant care
- **Database**: PostgreSQL database for storing plant data and sensor readings
- **Web Interface**: Angular-based dashboard for monitoring and control
- **IoT Integration**: OpenHAB compatibility for home automation

## 🛠️ System Requirements

### Hardware
- **Raspberry Pi 5** (recommended) or compatible Linux system
- **Camera module** (Raspberry Pi Camera or USB camera)
- **Stepper motors** (if using motor control features)
- **GPIO sensors** (soil moisture, temperature, etc.)

### ⚠️ Hardware Disclaimer
**This system requires actual hardware components to function properly. Software-only testing will have limited functionality:**

- **Camera Features**: Will not work without a physical camera device
- **GPIO Operations**: Will fail without Raspberry Pi or compatible GPIO hardware
- **Motor Control**: TMC2209 drivers require actual stepper motors and hardware
- **Sensor Data**: No real-time data without connected sensors
- **Streaming**: Video streaming requires camera hardware and sufficient processing power

**For development/testing without hardware:**
- Use the test projects to verify individual components
- Mock hardware interfaces for development
- Consider using emulators or virtual environments for basic testing

### Software Dependencies
- **.NET SDK 8.0** - Core runtime and development framework
- **OpenHAB** - Home automation platform (optional)
- **OpenCV** - Computer vision library
- **TensorFlow** - Machine learning framework
- **PostgreSQL** - Database system
- **Node.js & npm** - For Angular client development

## 📋 Prerequisites

### 0. Hardware Components
**Before proceeding with software setup, ensure you have the required hardware:**

- **Raspberry Pi 5** (or compatible single-board computer)
- **Camera Module** (Raspberry Pi Camera v3 or USB webcam)
- **Power Supply** (5V/3A minimum for Pi 5)
- **MicroSD Card** (32GB+ Class 10 recommended)
- **Stepper Motors** (if using motor control features)
- **GPIO Sensors** (soil moisture, temperature, humidity)
- **Motor Drivers** (TMC2209 or compatible)
- **Breadboard & Jumper Wires** for prototyping

**Note**: This system is designed for physical deployment. Software-only testing will have severely limited functionality.

### 1. .NET SDK 8.0
```bash
# Install .NET 8.0 SDK
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### 2. PostgreSQL Database
```bash
# Install PostgreSQL
sudo apt-get install postgresql postgresql-contrib

# Create database and user
sudo -u postgres psql
CREATE DATABASE senior_design;
CREATE USER pi WITH PASSWORD 'Fall2024';
GRANT ALL PRIVILEGES ON DATABASE senior_design TO pi;
\q
```

### 3. OpenHAB (Optional)
```bash
# Install OpenHAB
wget -qO - 'https://openhab.jfrog.io/artifactory/api/gpg/key/public' | sudo apt-key add -
echo 'deb https://openhab.jfrog.io/artifactory/openhab-linuxpkg stable main' | sudo tee /etc/apt/sources.list.d/openhab.list
sudo apt-get update
sudo apt-get install openhab
```

### 4. Node.js and npm
```bash
# Install Node.js 18+ and npm
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs
```

## 🔧 Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd Senior-design
```

### 2. Setup OpenCV (Linux/Raspberry Pi)
```bash
# Run the OpenCV setup script
cd ImagingDriver/Scripts
chmod +x SetupOpenCvSharp_linux.sh
./SetupOpenCvSharp_linux.sh
```

### 3. Setup RAM Disk for Streaming
```bash
# Setup RAM disk for video streaming
chmod +x SetupRamDisk.sh
./SetupRamDisk.sh
```

### 4. Install Dependencies
```bash
# Restore .NET packages
dotnet restore

# Install Angular CLI globally
npm install -g @angular/cli

# Install client dependencies
cd seniordesignfall2024.client
npm install
```

### 5. Choose Your Development Environment

#### Option A: Visual Studio (Windows)
- **Recommended for**: Windows users, full IDE experience
- **Requirements**: Visual Studio 2022 Community/Professional/Enterprise
- **Setup**: 
  - Install Visual Studio 2022 with ".NET desktop development" workload
  - Open `SeniorDesignFall2024.sln` in Visual Studio
  - Build solution with `Ctrl+Shift+B`
  - Run with `F5` (debug) or `Ctrl+F5` (without debug)

#### Option B: VS Code (Cross-platform)
- **Recommended for**: Linux, macOS, or lightweight Windows development
- **Requirements**: VS Code with C# extension
- **Setup**:
  ```bash
  # Install VS Code extensions
  code --install-extension ms-dotnettools.csharp
  code --install-extension ms-dotnettools.vscode-dotnet-runtime
  
  # Open project in VS Code
  code .
  ```
- **Usage**: 
  - Use `Ctrl+Shift+P` → "Run Task" → "build" to build
  - Use integrated terminal: `dotnet run` in project directories
  - Debug with F5 (requires launch.json configuration)

### 6. Configure Database
```bash
# Navigate to Database project
cd ../Database

# Create initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## 🚀 Running the Application

### Option 1: Command Line (Recommended for VS Code/Linux)
#### 1. Start the Server
```bash
# From the root directory
cd SeniorDesignFall2024.Server
dotnet run
```

#### 2. Start the Client (in a new terminal)
```bash
# From the client directory
cd seniordesignfall2024.client
npm start
```

### Option 2: Visual Studio (Windows)
#### 1. Start the Server
- Open `SeniorDesignFall2024.sln` in Visual Studio
- Set `SeniorDesignFall2024.Server` as startup project (right-click → "Set as Startup Project")
- Press `F5` to run with debugger or `Ctrl+F5` to run without debugger

#### 2. Start the Client
- In a new terminal or command prompt:
```bash
cd seniordesignfall2024.client
npm start
```

### 3. Access the Application
- **Server API**: https://localhost:7000
- **Client App**: https://localhost:4200
- **API Documentation**: https://localhost:7000/swagger

### ⚠️ Limited Functionality Without Hardware
**When running without physical hardware, expect these limitations:**
- Camera streaming endpoints will return errors
- GPIO operations will fail
- Motor control commands will not execute
- Sensor data will be static or missing
- Some API endpoints may return hardware-related errors

**What WILL work without hardware:**
- Web interface and navigation
- Database operations (if configured)
- Basic API structure and routing
- Configuration management
- Business logic components

## ⚙️ Configuration

### Server Configuration
Edit `SeniorDesignFall2024.Server/appsettings.json` to configure:

- **Database connection** (PostgreSQL)
- **Camera settings** (resolution, framerate)
- **GPIO pins** for sensors and motors
- **OpenHAB integration** settings
- **Streaming configuration**

### Key Configuration Options
```json
{
  "StartupConfig": {
    "EnableZwaveOpenHab": false,
    "EnableTmcDriver": false,
    "EnableDb": true,
    "EnableGpio": true,
    "EnableImaging": true,
    "EnableCamera": true
  },
  "StreamConfig": {
    "UseRpiCamInput": true,
    "OutputWidth": 1536,
    "OutputHeight": 864,
    "OutputFramerate": 24
  }
}
```

## 🧪 Testing

### Run Unit Tests
```bash
# Test all projects
dotnet test

# Test specific project
dotnet test MachineVisionTest/
dotnet test ImagingDriverTest/
dotnet test DatabaseTest/
dotnet test TmcTest/
```

### Hardware Simulation & Testing
Since this system requires physical hardware, consider these testing approaches:

- **Unit Tests**: Test individual components and business logic
- **Integration Tests**: Test component interactions (when hardware is available)
- **Mock Hardware**: Use the test projects to simulate hardware responses
- **Hardware Emulation**: Some components can be tested with virtual devices
- **Partial Testing**: Test database, web interface, and business logic without hardware

### Test Individual Components
```bash
# Test Machine Vision
cd MachineVisionTest
dotnet run

# Test Imaging Driver
cd ImagingDriverTest
dotnet run

# Test TMC Driver
cd TmcTest
dotnet run
```

## 🐛 Debugging & Development

### Visual Studio Debugging
- **Breakpoints**: Click in the left margin or press `F9`
- **Step Through**: `F10` (step over), `F11` (step into), `Shift+F11` (step out)
- **Watch Window**: Add variables to watch during debugging
- **Immediate Window**: Execute code while paused at breakpoints

### VS Code Debugging
- **Breakpoints**: Click in the left margin or press `F9`
- **Debug Console**: Execute code while paused at breakpoints
- **Variables Panel**: Inspect local and global variables
- **Call Stack**: Navigate through function calls

### Development Workflow Differences

| Feature | Visual Studio | VS Code |
|---------|---------------|---------|
| **IntelliSense** | Full-featured, real-time | Good with C# extension |
| **Debugging** | Advanced debugging tools | Basic debugging support |
| **Build System** | Integrated MSBuild | Command line (`dotnet build`) |
| **Package Management** | NuGet Package Manager | Command line (`dotnet add package`) |
| **Solution Explorer** | Built-in tree view | File explorer with project structure |
| **Performance** | Heavier, more features | Lightweight, faster startup |

## 📁 Project Structure

```
Senior-design/
├── SeniorDesignFall2024.Server/          # Main ASP.NET Core server
├── seniordesignfall2024.client/          # Angular web client
├── MachineVision/                        # TensorFlow & OpenCV operations
├── ImagingDriver/                        # Camera and streaming services
├── Database/                             # Entity Framework & PostgreSQL
├── TmcDriver/                            # Stepper motor control
├── ImportAnomaly/                        # Anomaly detection models
└── SeniorDesignFall2024.Server.Shared/  # Shared configuration
```

## 🔍 Troubleshooting

### Platform-Specific Considerations

#### Windows Development
- **Visual Studio**: Use the full IDE for best experience
- **VS Code**: Install C# extension and .NET runtime extension
- **Dependencies**: Most packages are automatically managed by NuGet

#### Linux/Raspberry Pi Development
- **VS Code**: Recommended for lightweight development
- **Command Line**: Use `dotnet` CLI for all operations
- **Dependencies**: Manual installation required for native libraries

#### macOS Development
- **VS Code**: Primary development environment
- **Command Line**: Use `dotnet` CLI and Homebrew for dependencies

### Common Issues

1. **OpenCV Installation Fails**
   - Ensure you have build tools: `sudo apt-get install build-essential cmake`
   - Check available memory: OpenCV compilation requires significant RAM

2. **Database Connection Issues**
   - Verify PostgreSQL is running: `sudo systemctl status postgresql`
   - Check connection string in `appsettings.json`
   - Ensure database user has proper permissions

3. **Camera Not Working**
   - Check camera permissions: `sudo usermod -a -G video $USER`
   - Verify camera is detected: `ls /dev/video*`
   - Test with `v4l2-ctl --list-devices`

4. **GPIO Access Denied**
   - Add user to gpio group: `sudo usermod -a -G gpio $USER`
   - Reboot or log out/in for changes to take effect

### Performance Optimization

- **RAM Disk**: Use the provided script for optimal streaming performance
- **Camera Resolution**: Adjust in `appsettings.json` based on your needs
- **Database**: Consider connection pooling for high-traffic scenarios

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is part of a senior design course. Please refer to your course guidelines for usage and distribution terms.

## 🆘 Support

For issues and questions:
1. Check the troubleshooting section above
2. Review the configuration files
3. Check the test projects for usage examples
4. Consult the API documentation at `/swagger`

---

**Note**: This system is designed for educational and research purposes. Ensure proper safety measures when working with electrical components and motors.
