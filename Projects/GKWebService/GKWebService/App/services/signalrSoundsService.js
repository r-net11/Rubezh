(function () {

	var app = angular.module('gkApp.services')
        .factory('signalrSoundsService', ['Hub', 'broadcastService', '$cookies', function (Hub, broadcastService, $cookies) {

        	var soundService = {

        		getIsSound: function () {
        			return $cookies.get('isSound') == 'true';
        		},

        		setIsSound: function (isSound) {
        			$cookies.put('isSound', isSound);
        		},

        		onSoundPlay: function (func) {
        			broadcastService.on('signalrSoundsService.soundPlay', func);
        		},

        		sounds: {},

        		play: function (sound) {
        			if (!this.sounds[sound]) {
        				this.sounds[sound] = new Audio('/Content/Sounds/' + sound);
        			}
        			this.sounds[sound].play();

        			broadcastService.send('signalrSoundsService.soundPlay');
        		}
        	};

        	var devicesHub = new Hub('soundsUpdater', {
        		listeners: {
        			'updateSounds': function (data) {
        				soundService.setIsSound(true);
        				soundService.play(data.sound.SoundName)
        			}
        		}
        	});

        	return soundService;
        }]);
}());