#include <fstream>
#include <Vorbis/codec.h>
#include <Vorbis/vorbisfile.h>

#define DLL_EXPORT extern "C" __declspec(dllexport)

/**
 * Wavのファイルヘッダーフォーアット
 */
struct WavFileHeader{
	int8_t		riff[ 4 ];		//!< 'R''I''F''F'
	uint32_t	size;			//!< all-8
	int8_t		wave[ 4 ];		//!< 'W''A''V''E'
	int8_t		fmt[ 4 ];		//!< 'f''m''t'' '
	uint32_t	fmtSize;		//!< fmtチャンクサイズ(16固定)
	uint16_t	formatId;		//!< フォーマットID
	uint16_t	channel;		//!< チャンネル数
	uint32_t	samplingRate;	//!< サンプリングレート(Hz)
	uint32_t	bps;			//!< バイト/秒
	uint16_t	blockSize;		//!< ブロックサイズ
	uint16_t	bitSize;		//!< ビット数
	int8_t		data[ 4 ];		//!< 'd''a''t''a'
	uint32_t	pcmSize;		//!< PCMデータサイズ
};

const uint32_t kQuantizationSize = sizeof(int16_t);	// 量子化サイズ

bool initialized = false;
OggVorbis_File ovFile;

DLL_EXPORT bool __stdcall OggInitialize(char* name);
DLL_EXPORT void __stdcall OggFinalize();
DLL_EXPORT uint32_t __stdcall OggPcmSize();
DLL_EXPORT uint32_t __stdcall OggExpandSize();

DLL_EXPORT bool __stdcall OggInitialize( char* name )
{
	OggFinalize();

	if( ov_fopen(name, &ovFile) != 0 ){
		return false;
	}
	initialized = true;
	return true;
}

DLL_EXPORT void __stdcall OggFinalize()
{
	if( initialized )
	{
		ov_clear(&ovFile);
		initialized = false;
	}
}

DLL_EXPORT uint32_t __stdcall OggPcmSize()
{
	if( initialized )
	{
		vorbis_info* pInfo = ov_info(&ovFile, -1);
		uint32_t pcmSize = static_cast<uint32_t>(ov_pcm_total(&ovFile, -1));
		return pcmSize * pInfo->channels * kQuantizationSize;
	}
	return 0;
}

DLL_EXPORT uint32_t __stdcall OggExpandSize()
{
	if( initialized )
	{
		return OggPcmSize() + sizeof(WavFileHeader);
	}
	return 0;
}

DLL_EXPORT void __stdcall OggDecode( int8_t* pOutputBuffer )
{
	if( !initialized || !pOutputBuffer ) return;

	vorbis_info* pInfo = ov_info(&ovFile, -1);

	uint32_t bufferSize = OggExpandSize();
	uint32_t pcmSize = OggPcmSize();
	WavFileHeader* fileHeader = reinterpret_cast<WavFileHeader*>(pOutputBuffer);

	fileHeader->riff[0] = 'R';
	fileHeader->riff[1] = 'I';
	fileHeader->riff[2] = 'F';
	fileHeader->riff[3] = 'F';
	fileHeader->size = bufferSize - 8;
	fileHeader->wave[0] = 'W';
	fileHeader->wave[1] = 'A';
	fileHeader->wave[2] = 'V';
	fileHeader->wave[3] = 'E';
	fileHeader->fmt[0] = 'f';
	fileHeader->fmt[1] = 'm';
	fileHeader->fmt[2] = 't';
	fileHeader->fmt[3] = ' ';
	fileHeader->fmtSize = 16;
	fileHeader->formatId = 1;
	fileHeader->channel = static_cast<uint16_t>(pInfo->channels);
	fileHeader->samplingRate = pInfo->rate;
	fileHeader->bps = pInfo->rate * pInfo->channels * kQuantizationSize;
	fileHeader->blockSize = static_cast<uint16_t>(kQuantizationSize * fileHeader->channel);
	fileHeader->bitSize = static_cast<uint16_t>(kQuantizationSize * 8);
	fileHeader->data[0] = 'd';
	fileHeader->data[1] = 'a';
	fileHeader->data[2] = 't';
	fileHeader->data[3] = 'a';
	fileHeader->pcmSize = pcmSize;

	pOutputBuffer += sizeof(WavFileHeader);

	uint32_t readTotalSize = 0;
	while( pcmSize > readTotalSize )
	{
		long readSize = ov_read(&ovFile, reinterpret_cast<char*>(pOutputBuffer), pcmSize - readTotalSize, 0, 2, 1, nullptr);
		if( readSize <= 0 ){
			break;
		}
		pOutputBuffer += readSize;
		readTotalSize += readSize;
	}
}
