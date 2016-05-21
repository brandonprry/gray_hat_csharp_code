/*  Hive.c
    Hive file access, pretty lame and bugged but do the work O_o
    Thanks to B.D. for file structure info
    
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
    
    This program is released under the GPL with the additional exemption 
    that compiling, linking, and/or using OpenSSL is allowed.

    Copyright (C) 2004-2006 Nicola Cuomo <ncuomo@studenti.unina.it>
    Improvments and some bugs fixes by Objectif Securité
    <http://www.objectif-securite.ch>
*/

#include <sys/param.h>
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



nk_hdr* read_nk(nk_hdr *nk, struct hive *h, int offset ) {
  memcpy(nk, h->base+offset+4, sizeof(nk_hdr));
  nk->key_name = (h->base+offset+4+76);
#if BYTE_ORDER == LITTLE_ENDIAN
#elif BYTE_ORDER == BIG_ENDIAN
  nk->id = __bswap_16(nk->id);
  nk->type = __bswap_16(nk->type);
  nk->name_len = __bswap_16(nk->name_len);
  nk->classname_len = __bswap_16(nk->classname_len);
  nk->classname_off = __bswap_32(nk->classname_off);
  nk->unk2 = __bswap_32(nk->unk2);
  nk->lf_off = __bswap_32(nk->lf_off);
  nk->value_cnt = __bswap_32(nk->value_cnt);
  nk->value_off = __bswap_32(nk->value_off);
  nk->subkey_num = __bswap_32(nk->subkey_num);
#endif
  return nk;
}

lf_hdr* read_lf(lf_hdr *lf, struct hive *h, int offset ) {
  memcpy(lf, h->base+offset+4, sizeof(lf_hdr));
  lf->hr = (h->base+offset+4+4);
#if BYTE_ORDER == LITTLE_ENDIAN
#elif BYTE_ORDER == BIG_ENDIAN
  lf->id = __bswap_16(lf->id);
  lf->key_num = __bswap_16(lf->key_num);
#endif
  return lf;
}

vk_hdr* read_vk(vk_hdr *vk, struct hive *h, int offset ) {
  memcpy(vk, h->base+offset+4, sizeof(vk_hdr));
  vk->value_name = (h->base+offset+4+20);
#if BYTE_ORDER == LITTLE_ENDIAN
#elif BYTE_ORDER == BIG_ENDIAN
  vk->id = __bswap_16(vk->id);
  vk->name_len = __bswap_16(vk->name_len);
  vk->flag = __bswap_16(vk->flag);
  vk->data_len = __bswap_32(vk->data_len);
  vk->data_off = __bswap_32(vk->data_off);
  vk->data_type = __bswap_32(vk->data_type);
#endif
  return vk;
}

int* read_valuelist(int *value, struct hive *h, int offset, int size ){
  int i =0;
  memcpy(value, h->base+offset+4, size*sizeof(int));
#if BYTE_ORDER == LITTLE_ENDIAN
#elif BYTE_ORDER == BIG_ENDIAN
  for (i=0; i<size; i++)
    value[i] = __bswap_32(value[i]);
#endif
  return value;
}

hashrecord* read_hr(hashrecord *hr, unsigned char *pos, int index ) {
  pos+=(8*index);
  memcpy(hr, pos, sizeof(hashrecord));
#if BYTE_ORDER == LITTLE_ENDIAN
#elif BYTE_ORDER == BIG_ENDIAN
  hr->nk_offset = __bswap_32(hr->nk_offset);
#endif    
  return hr;
}


unsigned char* read_data(struct hive *h, int offset ) {
  return ( (unsigned char*) (h->base + offset + 4)  );
}


void _RegCloseHive( struct hive *h ) {
  if( h->base != NULL )
    free( h->base );
  return;
}

void _InitHive( struct hive *h ) {
  h->base = NULL;
  return;
}

int _RegOpenHive( char *filename, struct hive *h ) {
  FILE *hiveh;
  unsigned long hsize;
  
  /* Prova ad aprire l'hive */
  if( ( hiveh = fopen( filename, "rb" ) ) != NULL ) {
    if( fseek( hiveh, 0, SEEK_END ) == 0 ) {
      hsize = ftell( hiveh );
      
      /* Legge il file in memoria */
      /* MMF ??? -_- */
      h->base = (unsigned char *) malloc( hsize );
      
      fseek( hiveh, 0, SEEK_SET );
      
      if( fread( (void *) h->base, hsize, 1, hiveh ) == 1 ){
#if BYTE_ORDER == LITTLE_ENDIAN
	if( *((int*)h->base) == 0x66676572 ) {
	  fclose( hiveh );				
	  return 0;
	}
#elif BYTE_ORDER == BIG_ENDIAN
	if( *((int*)h->base) == __bswap_32( 0x66676572)) { 
	  fclose( hiveh );				
	  return 0;
	}
#endif
      }
    }
    fclose( hiveh );
  }
  return -1;
}

long parself( struct hive *h, char *t, unsigned long off ) {
  nk_hdr *n;
  lf_hdr *l;
  hashrecord *hr;
  long res;
  
  int i;
  
  hr = (hashrecord*) malloc(sizeof(hashrecord));
  n = (nk_hdr*) malloc(sizeof(nk_hdr));
  l = (lf_hdr*) malloc(sizeof(lf_hdr));
  l = read_lf(l, h, off );
#ifdef DEBUG
  printf("off = %x, key_num = %d ", off, l->key_num);
#endif
  
  for( i = 0; i < l->key_num; i++ ){
    hr = read_hr(hr, l->hr, i);
    n = read_nk(n, h, hr->nk_offset + 0x1000 );
#ifdef DEBUG
    printf("/ i=%d/%d, keyname = %x, nk_offset = %x\n", i, l->key_num, hr->keyname, hr->nk_offset);
    printf("//t = %s, keyname = %s, len = %d\n", t, strndup(n->key_name, n->name_len), n->name_len);
#endif
    if( !memcmp( t, n->key_name, n->name_len ) && (strlen(t) == n->name_len)) {
      res = hr->nk_offset;
      free(n);
      free(l);
      return res;
    }
  }
  free(n);
  free(l);
  return -1;
}

int _RegGetRootKey(struct hive *h, char **root_key) {
   nk_hdr *n;

   n = (nk_hdr*) malloc(sizeof(nk_hdr));
   n = read_nk(n, h, 0x1020);

#ifdef DEBUG
  printf("id = %x type = %x name_len = %d\n", n->id, n->type, n->name_len);
#endif
   
   if ( n->id == NK_ID && n->type == NK_ROOT ) {
     *root_key = (char *)malloc(n->name_len+1);
     strncpy(*root_key, n->key_name, n->name_len);
     (*root_key)[n->name_len] = 0;
     free(n);
     return 0;
   }
   free(n);
   return -1;
}

int _RegOpenKey( struct hive *h, char *path, nk_hdr **nr ) {
  nk_hdr *n;
  char *t, *tpath;
  unsigned long noff;
  
  n = (nk_hdr*) malloc(sizeof(nk_hdr));
  n = read_nk(n, h, 0x1020 );

#ifdef DEBUG
  printf("id = %x type = %x\n", n->id, n->type);
#endif
  if( n->id == NK_ID && n->type == NK_ROOT ) {
    tpath = strdup( path );
    t = strtok( tpath, "\\" );
#ifdef DEBUG
    printf("1 %s\n", t);
#endif
    if( !memcmp( t, n->key_name, n->name_len ) ) {
      t = strtok( NULL, "\\" );
#ifdef DEBUG
      printf("2 %s %x %x\n", t, n->unk2, n->lf_off);
#endif
      while( t != NULL ) {
	if( ( noff = parself( h, t, n->lf_off + 0x1000 ) ) == -1 ){
	  free(n);
	  return -1;
	}
	n = read_nk(n, h, noff + 0x1000 );
	t = strtok( NULL, "\\" );
#ifdef DEBUG
	printf("t = %s, noff = %x\n", t, noff);
#endif
      }
      memcpy(*nr, n, sizeof(nk_hdr));
      free(n);
      return 0;
    }
    free( tpath );
  }
  free(n);
  return -1;
}

int _RegQueryValue( struct hive *h, char *name, nk_hdr *nr, unsigned char **buff, int *len ) {
  vk_hdr *v;
  unsigned int i,j;
  int *l;
  
  v = (vk_hdr*) malloc(sizeof(vk_hdr));
  l = (int*) malloc(sizeof(int)*nr->value_cnt);
  l = read_valuelist(l, h, nr->value_off + 0x1000, nr->value_cnt);
  
  *len = 0;
  
  for( i = 0; i < nr->value_cnt; i++ ) {
    v = read_vk(v, h, l[i] + 0x1000 );
    if( !memcmp( name, v->value_name, strlen( name ) ) || (name == NULL && ( v->flag & 1 ) == 0 ) ) {
#ifdef DEBUG
      printf("%s len=%x &off=%x off=%x type=%x\n", v->value_name, v->data_len&0x0000FFFF, &(v->data_off), v->data_off, v->data_type);
#endif
      *len =  v->data_len & 0x0000FFFF; 
      if (*buff!=NULL)
	free(*buff);
      *buff = (unsigned char*)malloc((*len));
      if ((*len)< 5 ) {
      	memcpy (*buff, &(v->data_off), (*len));
      }
      else
      	memcpy (*buff, read_data( h, v->data_off + 0x1000),(*len));
#ifdef DEBUG
      printf("buff = ", (*len));
      for (j=0; j<(*len); j++)
	printf("%.2x", (*buff)[j]);
      printf("\n");
#endif

      free(v);
      return 0;
    }
  }
  free(v);
  return -1;
}

int _RegEnumKey( struct hive *h, nk_hdr *nr, int index, char *name, int *namelen ) {
  lf_hdr *lf;
  nk_hdr *nk;
  hashrecord *hr;
  
  lf = (lf_hdr*) malloc(sizeof(lf_hdr));
  nk = (nk_hdr*) malloc(sizeof(nk_hdr));
  hr = (hashrecord*) malloc(sizeof(hashrecord));
#if DEBUG
  printf("subkey_num = %d\n", nr->subkey_num);
#endif

  if( index < nr->subkey_num ) {
    lf = read_lf(lf, h, nr->lf_off + 0x1000 );
    hr = read_hr(hr, lf->hr, index);
    nk = read_nk(nk, h, hr->nk_offset + 0x1000 );
    memcpy( name, nk->key_name, min( *namelen, nk->name_len ) );
    name[ min( *namelen, nk->name_len ) ] = 0;
    *namelen = nk->name_len;
    free(lf);
    return ( (index + 1) < nr->subkey_num ) ? (index + 1) : -1;
  }
  free(lf);
  return -1;
}
