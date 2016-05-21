/*  Hive.h
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

#ifndef HIVE_H
#define HIVE_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define min(a, b) (((a) < (b)) ? (a) : (b))

struct hive
{
  unsigned char *base;
};

typedef struct _nk_hdr 
{
  short int	id;
  short int	type;
  int	t1, t2;
  int	unk1;
  int	parent_off;
  int	subkey_num;
  int	unk2;
  int	lf_off;
  int	unk3;
  int	value_cnt;
  int	value_off;
  int	sk_off;
  int	classname_off;
  int	unk4[4];
  int	unk5;
  short int	name_len;
  short int	classname_len;
  unsigned char	*key_name; 
} nk_hdr;

typedef struct _hashrecord 
{
  
  int	nk_offset;
  char	keyname[4];
} hashrecord;

typedef struct _lf_hdr 
{
  short int	id;
  short int	key_num;
  unsigned char *hr;
} lf_hdr;

typedef struct _vk_hdr 
{
  short int  id;
  short int  name_len;
  int data_len;
  int data_off;
  int data_type;
  short int  flag;
  short int unk1;
  unsigned char *value_name;
} vk_hdr;

#define NK_ID	0x6B6E
#define NK_ROOT 0x2c

#define LF_ID	0x666C

nk_hdr* read_nk(nk_hdr *nk, struct hive *h, int offset ); 
lf_hdr* read_lf(lf_hdr *lf, struct hive *h, int offset );
vk_hdr* read_vk(vk_hdr *vk, struct hive *h, int offset );
hashrecord* read_hr(hashrecord *hr, unsigned char *pos, int index );
int* read_valuelist(int *value, struct hive *h, int offset, int size);
unsigned char* read_data(struct hive *h, int offset );


void _RegCloseHive( struct hive *h );

void _InitHive( struct hive *h );

int _RegOpenHive( char *filename, struct hive *h );

long parself( struct hive *h, char *t, unsigned long off );

int _RegGetRootKey(struct hive *h, char **root_key);

int _RegOpenKey( struct hive *h, char *path, nk_hdr **nr );

int _RegQueryValue( struct hive *h, char *name, nk_hdr *nr, unsigned char **buff, int *len );

int _RegEnumKey( struct hive *h, nk_hdr *nr, int index, char *name, int *namelen );

#endif
