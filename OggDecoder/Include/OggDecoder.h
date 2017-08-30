#pragma once

/*
 * @file OggDecoder.h
 * Oggデコーダー
 */

#include <cstdint>

extern "C"
{
	int8_t* OggDecode( const char* name );
} // extern "C"
