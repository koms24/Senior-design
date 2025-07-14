/*
* MATLAB Compiler: 24.1 (R2024a)
* Date: Sat Nov  2 13:56:01 2024
* Arguments:
* "-B""macro_default""-W""dotnet:MagicSquareComp,MagicSquareClass,4.0,private,version=1.0"
* "-T""link:lib""-d""C:\Users\robertstarkey\OneDrive\Documents\MATLAB\MagicSquareComp\for_
* testing""-v""class{MagicSquareClass:C:\Users\robertstarkey\OneDrive\Documents\MATLAB\mak
* esquare.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace MagicSquareComp
{

  /// <summary>
  /// The MagicSquareClass class provides a CLS compliant, MWArray interface to the
  /// MATLAB functions contained in the files:
  /// <newpara></newpara>
  /// C:\Users\robertstarkey\OneDrive\Documents\MATLAB\makesquare.m
  /// </summary>
  /// <remarks>
  /// @Version 1.0
  /// </remarks>
  public class MagicSquareClass : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static MagicSquareClass()
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

          string ctfFileName = "MagicSquareComp.ctf";

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
    /// Constructs a new instance of the MagicSquareClass class.
    /// </summary>
    public MagicSquareClass()
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
    ~MagicSquareClass()
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
    /// Provides a single output, 0-input MWArrayinterface to the makesquare MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// MAKESQUARE Magic square of size x.
    /// Y = MAKESQUARE(X) returns a magic square of size x.
    /// This file is used as an example for the MATLAB 
    /// .NET Assembly product.
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray makesquare()
    {
      return mcr.EvaluateFunction("makesquare", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the makesquare MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// MAKESQUARE Magic square of size x.
    /// Y = MAKESQUARE(X) returns a magic square of size x.
    /// This file is used as an example for the MATLAB 
    /// .NET Assembly product.
    /// </remarks>
    /// <param name="x">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray makesquare(MWArray x)
    {
      return mcr.EvaluateFunction("makesquare", x);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the makesquare MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// MAKESQUARE Magic square of size x.
    /// Y = MAKESQUARE(X) returns a magic square of size x.
    /// This file is used as an example for the MATLAB 
    /// .NET Assembly product.
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] makesquare(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "makesquare", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the makesquare MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// MAKESQUARE Magic square of size x.
    /// Y = MAKESQUARE(X) returns a magic square of size x.
    /// This file is used as an example for the MATLAB 
    /// .NET Assembly product.
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="x">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] makesquare(int numArgsOut, MWArray x)
    {
      return mcr.EvaluateFunction(numArgsOut, "makesquare", x);
    }


    /// <summary>
    /// Provides an interface for the makesquare function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// MAKESQUARE Magic square of size x.
    /// Y = MAKESQUARE(X) returns a magic square of size x.
    /// This file is used as an example for the MATLAB 
    /// .NET Assembly product.
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void makesquare(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("makesquare", numArgsOut, ref argsOut, argsIn);
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
