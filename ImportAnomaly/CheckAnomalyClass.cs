/*
* MATLAB Compiler: 24.1 (R2024a)
* Date: Sat Nov  2 14:09:31 2024
* Arguments:
* "-B""macro_default""-W""dotnet:checkAnomalyComp,CheckAnomalyClass,4.0,private,version=1.
* 0""-T""link:lib""-d""C:\Users\robertstarkey\OneDrive\Documents\MATLAB\MagicSquareComp\fo
* r_testing""-v""class{CheckAnomalyClass:C:\Users\robertstarkey\OneDrive\Documents\MATLAB\
* checkAnomaly.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace checkAnomalyComp
{

  /// <summary>
  /// The CheckAnomalyClass class provides a CLS compliant, MWArray interface to the
  /// MATLAB functions contained in the files:
  /// <newpara></newpara>
  /// C:\Users\robertstarkey\OneDrive\Documents\MATLAB\checkAnomaly.m
  /// </summary>
  /// <remarks>
  /// @Version 1.0
  /// </remarks>
  public class CheckAnomalyClass : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static CheckAnomalyClass()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

		  int lastDelimiter = ctfFilePath.LastIndexOf(@"/");

	      if (lastDelimiter == -1)
		  {
		    lastDelimiter = ctfFilePath.LastIndexOf(@"\");
		  }

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "checkAnomalyComp.ctf";

          Stream embeddedCtfStream = null;

          String[] resourceStrings = assembly.GetManifestResourceNames();

          foreach (String name in resourceStrings)
          {
            if (name.Contains(ctfFileName))
            {
              embeddedCtfStream = assembly.GetManifestResourceStream(name);
              break;
            }
          }
          mcr= new MWMCR("",
                         ctfFilePath, embeddedCtfStream, true);
        }
        catch(Exception ex)
        {
          ex_ = new Exception("MWArray assembly failed to be initialized", ex);
        }
      }
      else
      {
        ex_ = new ApplicationException("MWArray assembly could not be initialized");
      }
    }


    /// <summary>
    /// Constructs a new instance of the CheckAnomalyClass class.
    /// </summary>
    public CheckAnomalyClass()
    {
      if(ex_ != null)
      {
        throw ex_;
      }
    }


    #endregion Constructors

    #region Finalize

    /// <summary internal= "true">
    /// Class destructor called by the CLR garbage collector.
    /// </summary>
    ~CheckAnomalyClass()
    {
      Dispose(false);
    }


    /// <summary>
    /// Frees the native resources associated with this object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary internal= "true">
    /// Internal dispose function
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        disposed= true;

        if (disposing)
        {
          // Free managed resources;
        }

        // Free native resources
      }
    }


    #endregion Finalize

    #region Methods

    /// <summary>
    /// Provides a void output, 0-input MWArrayinterface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    ///
    public void checkAnomaly()
    {
      mcr.EvaluateFunction(0, "checkAnomaly", new MWArray[]{});
    }


    /// <summary>
    /// Provides a void output, 1-input MWArrayinterface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="historicalData">Input argument #1</param>
    ///
    public void checkAnomaly(MWArray historicalData)
    {
      mcr.EvaluateFunction(0, "checkAnomaly", historicalData);
    }


    /// <summary>
    /// Provides a void output, 2-input MWArrayinterface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="historicalData">Input argument #1</param>
    /// <param name="newReading">Input argument #2</param>
    ///
    public void checkAnomaly(MWArray historicalData, MWArray newReading)
    {
      mcr.EvaluateFunction(0, "checkAnomaly", historicalData, newReading);
    }


    /// <summary>
    /// Provides a void output, 3-input MWArrayinterface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="historicalData">Input argument #1</param>
    /// <param name="newReading">Input argument #2</param>
    /// <param name="k">Input argument #3</param>
    ///
    public void checkAnomaly(MWArray historicalData, MWArray newReading, MWArray k)
    {
      mcr.EvaluateFunction(0, "checkAnomaly", historicalData, newReading, k);
    }


    /// <summary>
    /// Provides a void output, 4-input MWArrayinterface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="historicalData">Input argument #1</param>
    /// <param name="newReading">Input argument #2</param>
    /// <param name="k">Input argument #3</param>
    /// <param name="anomalyThreshold">Input argument #4</param>
    ///
    public void checkAnomaly(MWArray historicalData, MWArray newReading, MWArray k, 
                       MWArray anomalyThreshold)
    {
      mcr.EvaluateFunction(0, "checkAnomaly", historicalData, newReading, k, anomalyThreshold);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] checkAnomaly(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "checkAnomaly", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="historicalData">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] checkAnomaly(int numArgsOut, MWArray historicalData)
    {
      return mcr.EvaluateFunction(numArgsOut, "checkAnomaly", historicalData);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="historicalData">Input argument #1</param>
    /// <param name="newReading">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] checkAnomaly(int numArgsOut, MWArray historicalData, MWArray 
                            newReading)
    {
      return mcr.EvaluateFunction(numArgsOut, "checkAnomaly", historicalData, newReading);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="historicalData">Input argument #1</param>
    /// <param name="newReading">Input argument #2</param>
    /// <param name="k">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] checkAnomaly(int numArgsOut, MWArray historicalData, MWArray 
                            newReading, MWArray k)
    {
      return mcr.EvaluateFunction(numArgsOut, "checkAnomaly", historicalData, newReading, k);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the checkAnomaly MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// Function to check if a new sensor reading is an anomaly based on historical data.
    /// Inputs:
    /// historicalData   - Matrix of historical readings (each row represents a reading
    /// from 5 sensors).
    /// newReading       - Row vector representing the new sensor reading to analyze.
    /// k                - Number of nearest neighbors to consider.
    /// anomalyThreshold - Threshold for determining if the new reading is an anomaly.
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="historicalData">Input argument #1</param>
    /// <param name="newReading">Input argument #2</param>
    /// <param name="k">Input argument #3</param>
    /// <param name="anomalyThreshold">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] checkAnomaly(int numArgsOut, MWArray historicalData, MWArray 
                            newReading, MWArray k, MWArray anomalyThreshold)
    {
      return mcr.EvaluateFunction(numArgsOut, "checkAnomaly", historicalData, newReading, k, anomalyThreshold);
    }



    /// <summary>
    /// This method will cause a MATLAB figure window to behave as a modal dialog box.
    /// The method will not return until all the figure windows associated with this
    /// component have been closed.
    /// </summary>
    /// <remarks>
    /// An application should only call this method when required to keep the
    /// MATLAB figure window from disappearing.  Other techniques, such as calling
    /// Console.ReadLine() from the application should be considered where
    /// possible.</remarks>
    ///
    public void WaitForFiguresToDie()
    {
      mcr.WaitForFiguresToDie();
    }



    #endregion Methods

    #region Class Members

    private static MWMCR mcr= null;

    private static Exception ex_= null;

    private bool disposed= false;

    #endregion Class Members
  }
}
