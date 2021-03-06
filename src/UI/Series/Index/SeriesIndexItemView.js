﻿'use strict';

define(
    [
        'vent',
        'marionette',
        'Commands/CommandController'
    ], function (vent, Marionette, CommandController) {
        return Marionette.ItemView.extend({

            ui: {
                refresh : '.x-refresh'
            },

            events: {
                'click .x-edit'    : '_editSeries',
                'click .x-refresh' : '_refreshSeries'
            },

            onRender: function () {
                CommandController.bindToCommand({
                    element: this.ui.refresh,
                    command: {
                        name     : 'refreshSeries',
                        seriesId : this.model.get('id')
                    }
                });
            },

            _editSeries: function () {
                vent.trigger(vent.Commands.EditSeriesCommand, {series: this.model});
            },

            _refreshSeries: function () {
                CommandController.Execute('refreshSeries', {
                    name    : 'refreshSeries',
                    seriesId: this.model.id
                });
            }
        });
    });
