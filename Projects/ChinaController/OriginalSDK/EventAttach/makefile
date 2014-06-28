#
# 2014-01-26
#

CXX = g++ 
CFLAGS = -Wl,-Bsymbolic -Wall -g
#CPPFLAGS = -Wl,-Bsymbolic -shared -fPIC -Wall -g
LDFLAGS = -L./
TARGET = eventAttach
LIBS = dhnetsdk dhdvr
SOURCES = $(wildcard *.cpp)
OBJECTS = $(patsubst %.cpp, %.o, $(SOURCES))
DEPENDS = $(patsubst %.cpp, %.d, $(SOURCES))

$(TARGET):$(OBJECTS)
	$(CXX) $(CFLAGS) $(OBJECTS) $(LDFLAGS) $(addprefix -l,$(LIBS)) -o $(TARGET)
	
.PHONY: clean
clean:
	rm -f $(OBJECTS) $(DEPENDS) $(TARGET) 
