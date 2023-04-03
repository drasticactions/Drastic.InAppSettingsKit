XBUILD=/Applications/Xcode_14.2.0.app/Contents/Developer/usr/bin/xcodebuild
ROOT=$(PWD)
PROJECT_ROOT=$(ROOT)/../external/InAppSettingsKit
PROJECT_IOS=$(PROJECT_ROOT)/InAppSettingsKit.xcodeproj
BUILD_ROOT=$(PROJECT_ROOT)/build
PROJECTNAME=BTViewState
FRAMEWORK_PATH=Products/@rpath/InAppSettingsKit.framework
FRAMEWORKS=$(ROOT)/Frameworks
TARGET_IOS="InAppSettingsKitFramework"
IOS_SDK=iphoneos16.2
IOS_BINDING_OUTPUT=$(ROOT)/../ios-bindings
HEADERS=$(FRAMEWORKS)/InAppSettingsKit.xcframework/ios-arm64/InAppSettingsKit.framework/Headers

build: iossimulator iosdevice maccatalyst xcframework

iossimulator:
	$(XBUILD) archive ONLY_ACTIVE_ARCH=NO -project $(PROJECT_IOS) -scheme $(TARGET_IOS) -configuration Debug -destination "generic/platform=iOS Simulator" -archivePath $(BUILD_ROOT)/simulator.xcarchive -sdk iphonesimulator SKIP_INSTALL=NO BUILD_LIBRARY_FOR_DISTRIBUTION=YES
	
iosdevice:
	$(XBUILD) archive ONLY_ACTIVE_ARCH=NO -project $(PROJECT_IOS) -scheme $(TARGET_IOS) -configuration Debug -destination "generic/platform=iOS" -archivePath $(BUILD_ROOT)/iOS.xcarchive -sdk iphoneos SKIP_INSTALL=NO BUILD_LIBRARY_FOR_DISTRIBUTION=YES

maccatalyst:
	$(XBUILD) archive ONLY_ACTIVE_ARCH=NO -project $(PROJECT_IOS) -scheme $(TARGET_IOS) -destination "generic/platform=macOS,variant=Mac Catalyst,name=Any Mac" -archivePath $(BUILD_ROOT)/catalyst.xcarchive SKIP_INSTALL=NO BUILD_LIBRARY_FOR_DISTRIBUTION=YES

xcframework:
	rm -rf $(BUILD_ROOT)/InAppSettingsKit.xcframework
	$(XBUILD) -create-xcframework -framework $(BUILD_ROOT)/simulator.xcarchive/$(FRAMEWORK_PATH) -framework $(BUILD_ROOT)/iOS.xcarchive/$(FRAMEWORK_PATH) -framework $(BUILD_ROOT)/catalyst.xcarchive/$(FRAMEWORK_PATH) -output $(BUILD_ROOT)/InAppSettingsKit.xcframework
	rm -rf $(FRAMEWORKS)
	mkdir -p $(FRAMEWORKS)
	cp -R $(BUILD_ROOT)/ $(FRAMEWORKS)

pack: 
	dotnet pack $(ROOT)/Drastic.InAppSettingsKit/Drastic.InAppSettingsKit.csproj --configuration Release --output $(ROOT)/../nuget

ios_sharpie:
	sharpie bind --sdk=$(IOS_SDK) --output="$(IOS_BINDING_OUTPUT)" --namespace="InAppSettingsKit" --scope="$(HEADERS)" $(HEADERS)/*.h