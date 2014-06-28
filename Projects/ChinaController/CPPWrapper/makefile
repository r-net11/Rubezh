#
# 2014-01-26
#

CXX = g++ 
CFLAGS = -Wl,-Bsymbolic -Wall -g
#CPPFLAGS = -Wl,-Bsymbolic -shared -fPIC -Wall -g
LDFLAGS = -L./
TARGET = AccessControlDemo
LIBS = pthread dhconfigsdk dhnetsdk dhdvr
LIBAS = #libavnetsdk.a libStreamSvr.a libStream.a libNetFramework.a libInfra.a libdhconfigsdk.a libdhnetsdk.a libdhdvr.a libdhTPLayer.a
SOURCES = $(wildcard *.cpp)
OBJECTS = $(patsubst %.cpp, %.o, $(SOURCES))
DEPENDS = $(patsubst %.cpp, %.d, $(SOURCES))

$(TARGET):$(OBJECTS)
	$(CXX) $(CFLAGS) $(OBJECTS) $(LDFLAGS) $(addprefix -l,$(LIBS)) $(LIBAS) -o $(TARGET)
	
.PHONY: clean
clean:
	rm -f $(OBJECTS) $(DEPENDS) $(TARGET) 
