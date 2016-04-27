﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 14.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Screna.Lame
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\Mathe_000\Documents\GitHub\Screna\Screna.Lame\LameFacade.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public partial class LameFacade : LameFacadeBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("using System;\r\nusing System.Runtime.InteropServices;\r\n\r\nnamespace Screna.Lame\r\n{\r" +
                    "\n    public class LameFacadeImpl\r\n    {\r\n        readonly IntPtr Context;\r\n     " +
                    "   bool IsClosed;\r\n\r\n        public LameFacadeImpl()\r\n        {\r\n            Con" +
                    "text = lame_init();\r\n            CheckResult(Context != IntPtr.Zero, \"lame_init\"" +
                    ");\r\n        }\r\n\r\n        ~LameFacadeImpl() { Dispose(); }\r\n\r\n        public void" +
                    " Dispose()\r\n        {\r\n            if (!IsClosed)\r\n            {\r\n              " +
                    "  lame_close(Context);\r\n                IsClosed = true;\r\n            }\r\n       " +
                    " }\r\n        \r\n        public int ChannelCount\r\n        {\r\n            get { retu" +
                    "rn lame_get_num_channels(Context); }\r\n            set { lame_set_num_channels(Co" +
                    "ntext, value); }\r\n        }\r\n\r\n        public int InputSampleRate\r\n        {\r\n  " +
                    "          get { return lame_get_in_samplerate(Context); }\r\n            set { lam" +
                    "e_set_in_samplerate(Context, value); }\r\n        }\r\n\r\n        public int OutputBi" +
                    "tRate\r\n        {\r\n            get { return lame_get_brate(Context); }\r\n         " +
                    "   set { lame_set_brate(Context, value); }\r\n        }\r\n\r\n        public int Outp" +
                    "utSampleRate { get { return lame_get_out_samplerate(Context); } }\r\n\r\n        pub" +
                    "lic int FrameSize { get { return lame_get_framesize(Context); } }\r\n\r\n        pub" +
                    "lic int EncoderDelay { get { return lame_get_encoder_delay(Context); } }\r\n\r\n    " +
                    "    public void PrepareEncoding()\r\n        {\r\n            // Set mode\r\n         " +
                    "   switch (ChannelCount)\r\n            {\r\n                case 1:\r\n              " +
                    "      lame_set_mode(Context, MpegMode.Mono);\r\n                    break;\r\n      " +
                    "          case 2:\r\n                    lame_set_mode(Context, MpegMode.Stereo);\r" +
                    "\n                    break;\r\n                default:\r\n                    Throw" +
                    "InvalidChannelCount();\r\n                    break;\r\n            }\r\n\r\n           " +
                    " // Disable VBR\r\n            lame_set_VBR(Context, VbrMode.Off);\r\n\r\n            " +
                    "// Prevent output of redundant headers\r\n            lame_set_write_id3tag_automa" +
                    "tic(Context, false);\r\n            lame_set_bWriteVbrTag(Context, 0);\r\n\r\n        " +
                    "    // Ensure not decoding\r\n            lame_set_decode_only(Context, 0);\r\n\r\n   " +
                    "         // Finally, initialize encoding process\r\n            CheckResult(lame_i" +
                    "nit_params(Context) == 0, \"lame_init_params\");\r\n        }\r\n\r\n        public int " +
                    "Encode(byte[] Source, int SourceOffset, int SampleCount, byte[] Destination, int" +
                    " DestinationOffset)\r\n        {\r\n            GCHandle SourceHandle = GCHandle.All" +
                    "oc(Source, GCHandleType.Pinned),\r\n                DestinationHandle = GCHandle.A" +
                    "lloc(Destination, GCHandleType.Pinned);\r\n\r\n            try\r\n            {\r\n     " +
                    "           IntPtr SourcePtr = new IntPtr(SourceHandle.AddrOfPinnedObject().ToInt" +
                    "64() + SourceOffset),\r\n                    DestinationPtr = new IntPtr(Destinati" +
                    "onHandle.AddrOfPinnedObject().ToInt64() + DestinationOffset);\r\n\r\n               " +
                    " int OutputSize = Destination.Length - DestinationOffset,\r\n                    R" +
                    "esult = -1;\r\n\r\n                switch (ChannelCount)\r\n                {\r\n       " +
                    "             case 1:\r\n                        Result = lame_encode_buffer(Contex" +
                    "t, SourcePtr, SourcePtr, SampleCount, DestinationPtr, OutputSize);\r\n            " +
                    "            break;\r\n                    case 2:\r\n                        Result " +
                    "= lame_encode_buffer_interleaved(Context, SourcePtr, SampleCount / 2, Destinatio" +
                    "nPtr, OutputSize);\r\n                        break;\r\n                    default:" +
                    "\r\n                        ThrowInvalidChannelCount();\r\n                        b" +
                    "reak;\r\n                }\r\n\r\n                CheckResult(Result >= 0, \"lame_encod" +
                    "e_buffer\");\r\n\r\n                return Result;\r\n            }\r\n            finall" +
                    "y\r\n            {\r\n                SourceHandle.Free();\r\n                Destinat" +
                    "ionHandle.Free();\r\n            }\r\n        }\r\n\r\n        public int FinishEncoding" +
                    "(byte[] Destination, int DestinationOffset)\r\n        {\r\n            var Destinat" +
                    "ionHandle = GCHandle.Alloc(Destination, GCHandleType.Pinned);\r\n\r\n            try" +
                    "\r\n            {\r\n                var DestinationPtr = new IntPtr(DestinationHand" +
                    "le.AddrOfPinnedObject().ToInt64() + DestinationOffset);\r\n\r\n                int D" +
                    "estinationLength = Destination.Length - DestinationOffset,\r\n                    " +
                    "Result = lame_encode_flush(Context, DestinationPtr, DestinationLength);\r\n\r\n     " +
                    "           CheckResult(Result >= 0, \"lame_encode_flush\");\r\n                retur" +
                    "n Result;\r\n            }\r\n            finally { DestinationHandle.Free(); }\r\n   " +
                    "     }\r\n\r\n        static void CheckResult(bool passCondition, string routineName" +
                    ")\r\n        {\r\n            if (!passCondition)\r\n                throw new Externa" +
                    "lException(string.Format(\"{0} failed\", routineName));\r\n        }\r\n\r\n        stat" +
                    "ic void ThrowInvalidChannelCount() { throw new InvalidOperationException(\"Set Ch" +
                    "annelCount to 1 or 2\"); }\r\n\r\n        #region LAME DLL API\r\n        const string " +
                    "DllName = \"lameenc");
            
            #line 145 "C:\Users\Mathe_000\Documents\GitHub\Screna\Screna.Lame\LameFacade.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Environment.Is64BitProcess ? 64 : 32));
            
            #line default
            #line hidden
            this.Write(".dll\";\r\n\r\n        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl" +
                    ")]\r\n        static extern IntPtr lame_init();\r\n\r\n        [DllImport(DllName, Cal" +
                    "lingConvention = CallingConvention.Cdecl)]\r\n        static extern int lame_close" +
                    "(IntPtr context);\r\n\r\n        [DllImport(DllName, CallingConvention = CallingConv" +
                    "ention.Cdecl)]\r\n        static extern int lame_set_in_samplerate(IntPtr context," +
                    " int value);\r\n\r\n        [DllImport(DllName, CallingConvention = CallingConventio" +
                    "n.Cdecl)]\r\n        static extern int lame_get_in_samplerate(IntPtr context);\r\n\r\n" +
                    "        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n     " +
                    "   static extern int lame_set_num_channels(IntPtr context, int value);\r\n\r\n      " +
                    "  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n        sta" +
                    "tic extern int lame_get_num_channels(IntPtr context);\r\n\r\n        enum MpegMode :" +
                    " int\r\n        {\r\n            Stereo = 0,\r\n            JointStereo = 1,\r\n        " +
                    "    DualChannel = 2,\r\n            Mono = 3,\r\n            NotSet = 4,\r\n        }\r" +
                    "\n\r\n        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n  " +
                    "      static extern int lame_set_mode(IntPtr context, MpegMode value);\r\n\r\n      " +
                    "  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n        sta" +
                    "tic extern MpegMode lame_get_mode(IntPtr context);\r\n\r\n        [DllImport(DllName" +
                    ", CallingConvention = CallingConvention.Cdecl)]\r\n        static extern int lame_" +
                    "set_brate(IntPtr context, int value);\r\n\r\n        [DllImport(DllName, CallingConv" +
                    "ention = CallingConvention.Cdecl)]\r\n        static extern int lame_get_brate(Int" +
                    "Ptr context);\r\n\r\n        [DllImport(DllName, CallingConvention = CallingConventi" +
                    "on.Cdecl)]\r\n        static extern int lame_set_out_samplerate(IntPtr context, in" +
                    "t value);\r\n\r\n        [DllImport(DllName, CallingConvention = CallingConvention.C" +
                    "decl)]\r\n        static extern int lame_get_out_samplerate(IntPtr context);\r\n\r\n  " +
                    "      [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n       " +
                    " static extern void lame_set_write_id3tag_automatic(IntPtr context, bool value);" +
                    "\r\n\r\n        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n " +
                    "       static extern bool lame_get_write_id3tag_automatic(IntPtr context);\r\n\r\n  " +
                    "      [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n       " +
                    " static extern int lame_set_bWriteVbrTag(IntPtr context, int value);\r\n\r\n        " +
                    "[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n        stati" +
                    "c extern int lame_get_bWriteVbrTag(IntPtr context);\r\n\r\n        enum VbrMode : in" +
                    "t\r\n        {\r\n            Off = 0,\r\n            MarkTaylor = 1,\r\n            Rog" +
                    "erHegemann = 2,\r\n            AverageBitRate = 3,\r\n            MarkTaylorRogerHeg" +
                    "emann = 4,\r\n        }\r\n\r\n        [DllImport(DllName, CallingConvention = Calling" +
                    "Convention.Cdecl)]\r\n        static extern int lame_set_VBR(IntPtr context, VbrMo" +
                    "de value);\r\n\r\n        [DllImport(DllName, CallingConvention = CallingConvention." +
                    "Cdecl)]\r\n        static extern VbrMode lame_get_VBR(IntPtr context);\r\n\r\n        " +
                    "[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n        stati" +
                    "c extern int lame_set_decode_only(IntPtr context, int value);\r\n\r\n        [DllImp" +
                    "ort(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n        static exter" +
                    "n int lame_get_decode_only(IntPtr context);\r\n\r\n        [DllImport(DllName, Calli" +
                    "ngConvention = CallingConvention.Cdecl)]\r\n        static extern int lame_get_enc" +
                    "oder_delay(IntPtr context);\r\n\r\n        [DllImport(DllName, CallingConvention = C" +
                    "allingConvention.Cdecl)]\r\n        static extern int lame_get_framesize(IntPtr co" +
                    "ntext);\r\n\r\n        [DllImport(DllName, CallingConvention = CallingConvention.Cde" +
                    "cl)]\r\n        static extern int lame_init_params(IntPtr context);\r\n\r\n        [Dl" +
                    "lImport(DllName, CallingConvention = CallingConvention.Cdecl)]\r\n        static e" +
                    "xtern int lame_encode_buffer(IntPtr context, IntPtr buffer_l, IntPtr buffer_r, i" +
                    "nt nsamples, IntPtr mp3buf, int mp3buf_size);\r\n\r\n        [DllImport(DllName, Cal" +
                    "lingConvention = CallingConvention.Cdecl)]\r\n        static extern int lame_encod" +
                    "e_buffer_interleaved(IntPtr context, IntPtr buffer, int nsamples, IntPtr mp3buf," +
                    " int mp3buf_size);\r\n\r\n        [DllImport(DllName, CallingConvention = CallingCon" +
                    "vention.Cdecl)]\r\n        static extern int lame_encode_flush(IntPtr context, Int" +
                    "Ptr mp3buf, int mp3buf_size);\r\n        #endregion\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public class LameFacadeBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
