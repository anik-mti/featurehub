# Dart Client SDK for FeatureHub


Welcome to the Dart implementation for FeatureHub. It is the supported version, but it does not mean
you cannot write your own, the functionality is quite straightforward.

![Code Coverage](coverage_badge.svg)

## Overview

This is the core library for Dart.

It provides the core functionality of the
repository which holds features and creates events. It depends on our own fork of the EventSourcing library
for Dart (until the main library merges the changes in or we release our own). 

This library only provides one recommended type of operation, which is that the server will update all the features
all of the time. _This is not appropriate for Mobile operation at this time as it will drain battery_. 

> This readme does not deal with the SDK capability of updating features while running your tests. That
capability is API client specific, and the sample we have is for Jersey. 

## Mechanisms for use

Like the Typescript SDK, there are four ways to use this library due to the more _user_ based interaction that your
application will operate under. Unlike the Typescript library, the Dart library uses _streams_.

### 1. All the Features, All the Time

In this mode, you will make a connection to the FeatureHub Edge server, and any updates to any events will come
through to you, updating the feature values in the repository. You can choose to listen for these updates and update
some UI immediate, but every `if` statement will use the updated value, so listening to the stream is usually a better choice.

A typical strategy will be:

- set up your UI state so it is in "loading" mode.
- listen to the readyness stream so you know when the features have loaded and you have an initial state for them. When
using a UI, this would lead to a transition to the actual UI rendering, on the server it would make it start listening
to the server port.
- set up your per feature stream listeners so you can react to their changes in state
- connect to the Feature Hub server. As soon as it has connected and received a list of features .
- each time a feature changes, it will call your listener and allow your application to react.

Whether this instant reaction is ideal for your application depends. For mobile and servers, the answer is usually
yes, for Web the answer is often no as people don't expect that.


````dart
final _repository = ClientFeatureRepository();

_repository.readynessStream.listen((readyness) {
  if (readyness == Readyness.Ready) {
    print("repo is ready to use");
  }
});

// this will cause the event source listener to immediately start. It has a close()
// method to allow for shutdown 
final _eventSource = EventSourceRepositoryListener(sdkUrl, _repository);

const featureXUnsubscribe = featureHubRepository.getFeatureState('FEATURE_X')
   .featureUpdateStream.listen((_fs) => do_something());
````

> Recommended for: servers

### 2. All the Features, Only Once

In this mode, you receive the connection and then you disconnect, ignoring any further connections. You would
use this mode only if you want to force the client to have a consistent UI experience through the lifetime of their
visit to your client application.

> Recommended for: Web only, and only when not intending to react to feature changes until you ask for the feature state again.

### 3. All the Features, Controlled Release

This mode is termed "catch-and-release" (yes, inspired by the Matt Simons song). It is intended to allow you get
an initial set of features but decide when the feature updates are released into your application.

A typical strategy would be:

. set up your UI state so it is in "loading" mode (only if in the Web).
. set up a readyness listener so you know when the features have loaded and you have an initial state for them. When
using a UI, this would lead to a transition to the actual UI rendering, on the server it would make it start listening
to the server port.
. tell the feature hub repository to operate in catch and release mode. `featureHubRepository.catchAndRelease = true;`
. listen to the new features stream. This stream will be triggered when a feature has changed.
. `[optional]` set up your per feature listeners so you can react to their changes in state. You could also not do this and
encourage you users to reload the whole application window (e.g. `window.location.reload()`).
. connect to the Feature Hub server. As soon as it has connected and received a list of features it will call your
readyness listener.
.


If you choose to not have listeners, when you call:

```dart
featureHubRepository.release();
```


then you should follow it with code to update your state with the appropriate changes in features. You
won't know which ones changed, but this can be a more efficient state update than using the listeners above.

## Failure

If for some reason the connection to the FeatureHub server fails - either initially or for some reason during
the process, you will get a readyness event to indicate that it has now failed.

```dart
enum Readyness {
  NotReady = 'NotReady',
  Ready = 'Ready',
  Failed = 'Failed'
}

```