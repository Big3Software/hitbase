/*============================================================================

       fourierd.c  -  Don Cross <dcross@intersrv.com>

       http://www.intersrv.com/~dcross/fft.html

       Contains definitions for doing Fourier transforms
       and inverse Fourier transforms.

       This module performs operations on arrays of 'double'.

============================================================================*/

#include <stdafx.h>
#include "hitmisc.h"
#include "fourier.h"
#include <math.h>

#define CHECKPOINTER(p)  CheckPointer(p,#p)

void CFFT::fft_double ( unsigned NumSamples,
                  int    InverseTransform,
                  double *RealIn,
                  double *ImagIn,
                  double *RealOut,
                  double *ImagOut )
{
   unsigned NumBits;    /* Number of bits needed to store indices */
   unsigned i, j, k, n;
   unsigned BlockSize, BlockEnd;

   double angle_numerator = 2.0 * DDC_PI;
   double delta_angle;
   double alpha, beta;  /* used in recurrence relation */
   double delta_ar;
   double tr, ti;     /* temp real, temp imaginary */
   double ar, ai;     /* angle vector real, angle vector imaginary */

   if ( !IsPowerOfTwo(NumSamples) )
   {
	   ASSERT(FALSE);         // NumSamples is not power of two
	   return;
   }

   if ( InverseTransform )
   {
      angle_numerator = -angle_numerator;
   }

   CHECKPOINTER ( RealIn );
   CHECKPOINTER ( RealOut );
   CHECKPOINTER ( ImagOut );

   NumBits = NumberOfBitsNeeded ( NumSamples );

   /*
   **   Do simultaneous data copy and bit-reversal ordering into outputs...
   */

   for ( i=0; i < NumSamples; i++ )
   {
      j = ReverseBits ( i, NumBits );

      RealOut[j] = RealIn[i];
      ImagOut[j] = (ImagIn == NULL) ? 0.0 : ImagIn[i];
   }

   /*
   **   Do the FFT itself...
   */

   BlockEnd = 1;
   for ( BlockSize = 2; BlockSize <= NumSamples; BlockSize <<= 1 )
   {
      delta_angle = angle_numerator / (double)BlockSize;
      alpha = sin ( 0.5 * delta_angle );
      alpha = 2.0 * alpha * alpha;
      beta = sin ( delta_angle );

      for ( i=0; i < NumSamples; i += BlockSize )
      {
         ar = 1.0;   /* cos(0) */
         ai = 0.0;   /* sin(0) */

         for ( j=i, n=0; n < BlockEnd; j++, n++ )
         {
            k = j + BlockEnd;
            tr = ar*RealOut[k] - ai*ImagOut[k];
            ti = ar*ImagOut[k] + ai*RealOut[k];

            RealOut[k] = RealOut[j] - tr;
            ImagOut[k] = ImagOut[j] - ti;

            RealOut[j] += tr;
            ImagOut[j] += ti;

            delta_ar = alpha*ar + beta*ai;
            ai -= (alpha*ai - beta*ar);
            ar -= delta_ar;
         }
      }

      BlockEnd = BlockSize;
   }

   /*
   **   Need to normalize if inverse transform...
   */

   if ( InverseTransform )
   {
      double denom = (double)NumSamples;

      for ( i=0; i < NumSamples; i++ )
      {
         RealOut[i] /= denom;
         ImagOut[i] /= denom;
      }
   }
}

#define BITS_PER_WORD   (sizeof(unsigned) * 8)

int CFFT::IsPowerOfTwo ( unsigned x )
{
   unsigned i, y;

   for ( i=1, y=2; i < BITS_PER_WORD; i++, y<<=1 )
   {
      if ( x == y ) return TRUE;
   }

   return FALSE;
}


unsigned CFFT::NumberOfBitsNeeded ( unsigned PowerOfTwo )
{
   unsigned i;

   if ( PowerOfTwo < 2 )
   {
	    ASSERT(FALSE);
		return 0;
   }

   for ( i=0; ; i++ )
   {
      if ( PowerOfTwo & (1 << i) )
      {
         return i;
      }
   }
}

unsigned CFFT::ReverseBits ( unsigned index, unsigned NumBits )
{
   unsigned i, rev;

   for ( i=rev=0; i < NumBits; i++ )
   {
      rev = (rev << 1) | (index & 1);
      index >>= 1;
   }

   return rev;
}


double CFFT::Index_to_frequency ( unsigned NumSamples, unsigned Index )
{
   if ( Index >= NumSamples )
   {
      return 0.0;
   }
   else if ( Index <= NumSamples/2 )
   {
      return (double)Index / (double)NumSamples;
   }
   else
   {
      return -(double)(NumSamples-Index) / (double)NumSamples;
   }
}

void CFFT::CheckPointer ( void *p, char *name )
{
   if ( p == NULL )
   {
      ASSERT(FALSE);  //fprintf ( stderr, "Error in fft_double():  %s == NULL\n", name );
      return;
   }
}

/*--- end of file fftmisc.c---*/
