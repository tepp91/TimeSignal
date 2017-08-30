using System.Runtime.InteropServices;

namespace TimeSignal.Models
{
	class OggDecoder
	{
		[DllImport( "OggDecoder.dll" )]
		[return: MarshalAs(UnmanagedType.U1)]
		public extern static bool OggInitialize(System.Text.StringBuilder path);

		[DllImport( "OggDecoder.dll" )]
		public extern static void OggFinalize();

		[DllImport( "OggDecoder.dll" )]
		public extern static uint OggExpandSize();

		[DllImport( "OggDecoder.dll" )]
		public extern static void OggDecode(byte[] outputBuffer);
	}
}
