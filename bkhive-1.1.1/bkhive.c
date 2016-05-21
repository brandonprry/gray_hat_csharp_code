/*  Bkhive 
    Extract Syskey bootkey from the system hive file
    
    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    
    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Copyright (C) 2004-2006 Nicola Cuomo <ncuomo@studenti.unina.it>
    Improvments and some bugs fixes by Objectif Securite
    <http://www.objectif-securite.ch>
*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "hive.h"

#ifdef BYTE_ORDER
#if BYTE_ORDER == LITTLE_ENDIAN
#elif BYTE_ORDER == BIG_ENDIAN
#include <byteswap.h>
#else
#warning "Doesn't define a standard ENDIAN type"
#endif
#else
#warning "Doesn't define BYTE_ORDER"
#endif

int main( int argc, char **argv ) {

  FILE *f;
  
  /* hive */
  struct hive h;
  nk_hdr *n = NULL;
  unsigned char *b;
  
  int i, j, buf_len, control;
  
  char *kn[] = { "JD", "Skew1", "GBG", "Data" };
  char kv[9];
  unsigned char *buf = NULL;
  char *keyname;
  char *root_key, *regselect, *reglsa;
  // System\ControlSet001\Control\Lsa\ on some nt4 box
  
  unsigned char key[0x10];
  unsigned char pkey[0x10];

#if BYTE_ORDER == LITTLE_ENDIAN
  int p[] = { 0xb, 0x6, 0x7, 0x1, 0x8, 0xa, 0xe, 0x0, 0x3, 0x5, 0x2, 0xf, 0xd, 0x9, 0xc, 0x4 };
#elif BYTE_ORDER == BIG_ENDIAN
  int p[] = { 0x8, 0x5, 0x4, 0x2, 0xb, 0x9, 0xd, 0x3, 0x0, 0x6, 0x1, 0xc, 0xe, 0xa, 0xf, 0x7 };
#endif
  
  printf( "bkhive 1.1.1 by Objectif Securite\nhttp://www.objectif-securite.ch\noriginal author: ncuomo@studenti.unina.it\n\n" );
  
  if( argc != 3 ) {
    printf( "Usage:\nbkhive systemhive keyfile\n" );
    return -1;
  }
  
  /* Initialize hive access */
  _InitHive( &h );
  
  /* Open the system hive file */
  if( _RegOpenHive( argv[1], &h ) ) {
    printf( "Error opening hive file %s\n", argv[1] );
    return -1;
  }
  
  /* Get Root key name 
     $$$PROTO.HIV for 2k/XP,
     CMI-CreateHive{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx} for Vista */
  if( _RegGetRootKey( &h, &root_key)) {
    printf( "Error reading hive root key\n");
    return -1;
  }
  printf("Root Key : %s\n", root_key);

  regselect = (char *) malloc(strlen(root_key)+10);
  reglsa = (char *) malloc(strlen(root_key)+33);

  sprintf(regselect, "%s\\Select", root_key);
  
  /* Find the Default ControlSet */
  n = (nk_hdr*) malloc(sizeof(nk_hdr));

  if (!_RegOpenKey(&h, regselect, &n)) 
    if (!_RegQueryValue( &h, "Default", n, &buf, &buf_len)) {
      if (buf_len == 4)
	control = *(int*)(buf);
      else
	control = 1;
      
      sprintf(reglsa,"%s\\ControlSet%03d\\Control\\Lsa\\", root_key, control);
      printf("Default ControlSet: %03d\n", control);fflush(stdout);
    }
    else {
      printf("Error reading ControlSet: _RegQueryValue\n");
      return -1;
    }
  else {
    printf("Error reading ControlSet: _RegOpenKey\n");
    return -1;
  }
  

  /* foreach keys */
  for( i = 0; i < 4; i++ ) {
    keyname = (char *) malloc( strlen( reglsa ) + strlen( kn[i] ) + 1 );
    sprintf( keyname, "%s%s", reglsa, kn[i] );
#ifdef DEBUG
    printf("********* %s *********\n", keyname);
#endif

    /* Access lsa subkey */
    if( _RegOpenKey( &h, keyname, &n ) )  {
      _RegCloseHive( &h );
      
      printf( "Error accessing key %s\nWrong/corrupted hive??\n", kn[i] );
      return -1;
    }
    
    /* Access the data */
    b = read_data( &h, n->classname_off + 0x1000 );
#ifdef DEBUG
    printf("n->classname_len = %d b = %x\n", n->classname_len, b-(h.base));
#endif

    // Quick hack for unicode -> ascii translation
    for( j = 0; j < n->classname_len && j<9; j++)
      kv[j] = b[j*2];
    kv[8] = 0;
    sscanf( kv, "%x", (int*)( &key[i*4] ) );

    free( keyname );
  }
  
  _RegCloseHive( &h );
  
#ifdef DEBUG
  printf("Bootkey unsorted: ");
  for( i = 0; i < 0x10; i++ )  {
    printf("%.2x", key[i]);
  }
  printf("\n");
#endif

  /* Print the boot key */
  printf( "Bootkey: " );
  
  for( i = 0; i < 0x10; i++ )  {
    /* Permute the class name */
    pkey[i] = key[p[i]];
    printf( "%.2x", pkey[i] );
  }
  
  printf( "\n" );
  
  /* write the syskey bootkey to file */
  if( ( f = fopen( argv[2], "wb" ) ) != NULL )  {
    fwrite( pkey, 1, 16, f );
    fclose( f );
  }
  else
    printf( "error writing to %s\n", argv[2] );
  
  free(n);
  return 0;
}
