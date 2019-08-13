# Microsoft Developer Studio Generated NMAKE File, Based on trix.dsp
!IF "$(CFG)" == ""
CFG=trix - Win32 Debug
!MESSAGE 構成が指定されていません。ﾃﾞﾌｫﾙﾄの trix - Win32 Debug を設定します。
!ENDIF 

!IF "$(CFG)" != "trix - Win32 Release" && "$(CFG)" != "trix - Win32 Debug"
!MESSAGE 指定された ﾋﾞﾙﾄﾞ ﾓｰﾄﾞ "$(CFG)" は正しくありません。
!MESSAGE NMAKE の実行時に構成を指定できます
!MESSAGE ｺﾏﾝﾄﾞ ﾗｲﾝ上でﾏｸﾛの設定を定義します。例:
!MESSAGE 
!MESSAGE NMAKE /f "trix.mak" CFG="trix - Win32 Debug"
!MESSAGE 
!MESSAGE 選択可能なﾋﾞﾙﾄﾞ ﾓｰﾄﾞ:
!MESSAGE 
!MESSAGE "trix - Win32 Release" ("Win32 (x86) Console Application" 用)
!MESSAGE "trix - Win32 Debug" ("Win32 (x86) Console Application" 用)
!MESSAGE 
!ERROR 無効な構成が指定されています。
!ENDIF 

!IF "$(OS)" == "Windows_NT"
NULL=
!ELSE 
NULL=nul
!ENDIF 

CPP=cl.exe
RSC=rc.exe

!IF  "$(CFG)" == "trix - Win32 Release"

OUTDIR=.\Release
INTDIR=.\Release
# Begin Custom Macros
OutDir=.\Release
# End Custom Macros

ALL : "$(OUTDIR)\trix.exe"


CLEAN :
	-@erase "$(INTDIR)\tri_algebra.obj"
	-@erase "$(INTDIR)\tri_geometry.obj"
	-@erase "$(INTDIR)\tri_main.obj"
	-@erase "$(INTDIR)\tri_triangulation.obj"
	-@erase "$(INTDIR)\tri_utility.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(OUTDIR)\trix.exe"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

CPP_PROJ=/nologo /ML /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_CONSOLE" /D "_MBCS" /Fp"$(INTDIR)\trix.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\trix.bsc" 
BSC32_SBRS= \
	
LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib  kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /incremental:no /pdb:"$(OUTDIR)\trix.pdb" /machine:I386 /out:"$(OUTDIR)\trix.exe" 
LINK32_OBJS= \
	"$(INTDIR)\tri_algebra.obj" \
	"$(INTDIR)\tri_geometry.obj" \
	"$(INTDIR)\tri_main.obj" \
	"$(INTDIR)\tri_triangulation.obj" \
	"$(INTDIR)\tri_utility.obj"

"$(OUTDIR)\trix.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "trix - Win32 Debug"

OUTDIR=.\Debug
INTDIR=.\Debug
# Begin Custom Macros
OutDir=.\Debug
# End Custom Macros

ALL : "$(OUTDIR)\trix.exe"


CLEAN :
	-@erase "$(INTDIR)\tri_algebra.obj"
	-@erase "$(INTDIR)\tri_geometry.obj"
	-@erase "$(INTDIR)\tri_main.obj"
	-@erase "$(INTDIR)\tri_triangulation.obj"
	-@erase "$(INTDIR)\tri_utility.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(OUTDIR)\trix.exe"
	-@erase "$(OUTDIR)\trix.ilk"
	-@erase "$(OUTDIR)\trix.pdb"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

CPP_PROJ=/nologo /MLd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_CONSOLE" /D "_MBCS" /Fp"$(INTDIR)\trix.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /GZ  /c 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\trix.bsc" 
BSC32_SBRS= \
	
LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib  kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /incremental:yes /pdb:"$(OUTDIR)\trix.pdb" /debug /machine:I386 /out:"$(OUTDIR)\trix.exe" /pdbtype:sept 
LINK32_OBJS= \
	"$(INTDIR)\tri_algebra.obj" \
	"$(INTDIR)\tri_geometry.obj" \
	"$(INTDIR)\tri_main.obj" \
	"$(INTDIR)\tri_triangulation.obj" \
	"$(INTDIR)\tri_utility.obj"

"$(OUTDIR)\trix.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ENDIF 

.c{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cpp{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cxx{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.c{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cpp{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cxx{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<


!IF "$(NO_EXTERNAL_DEPS)" != "1"
!IF EXISTS("trix.dep")
!INCLUDE "trix.dep"
!ELSE 
!MESSAGE Warning: cannot find "trix.dep"
!ENDIF 
!ENDIF 


!IF "$(CFG)" == "trix - Win32 Release" || "$(CFG)" == "trix - Win32 Debug"
SOURCE=.\tri_algebra.cpp

"$(INTDIR)\tri_algebra.obj" : $(SOURCE) "$(INTDIR)"


SOURCE=.\tri_geometry.cpp

"$(INTDIR)\tri_geometry.obj" : $(SOURCE) "$(INTDIR)"


SOURCE=.\tri_main.cpp

"$(INTDIR)\tri_main.obj" : $(SOURCE) "$(INTDIR)"


SOURCE=.\tri_triangulation.cpp

"$(INTDIR)\tri_triangulation.obj" : $(SOURCE) "$(INTDIR)"


SOURCE=.\tri_utility.cpp

"$(INTDIR)\tri_utility.obj" : $(SOURCE) "$(INTDIR)"



!ENDIF 

