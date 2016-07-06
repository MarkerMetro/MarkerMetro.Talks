package com.markermetro.aucklandgdg;

import android.app.AlarmManager;
import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.TaskStackBuilder;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.support.v4.app.NotificationCompat;
import android.util.Log;


public class RecommendationService extends IntentService {
    private static final long INITIAL_DELAY = 5000;

    private NotificationManager _notificationManager;

    public RecommendationService(){
        super("RecommendationService");
    }

    @Override
    protected void onHandleIntent(Intent intent) {
        if (_notificationManager == null) {
            _notificationManager = (NotificationManager) getApplicationContext().getSystemService(Context.NOTIFICATION_SERVICE);
        }

        Bitmap bm = BitmapFactory.decodeResource(getResources(), R.drawable.dk);

        Notification notification = new NotificationCompat.BigPictureStyle(
                new NotificationCompat.Builder(getApplicationContext())
                        .setAutoCancel(true)
                        .setContentTitle("Hello")
                        .setContentText("DK was here")
                        .setPriority(1)
                        .setLocalOnly(true)
                        .setOngoing(true)
                        .setColor(getResources().getColor(R.color.default_background))
                        .setCategory(Notification.CATEGORY_RECOMMENDATION)
                        .setLargeIcon(bm)
                        .setSmallIcon(R.drawable.mm_white)
                        .setContentIntent(buildPendingIntent())
                        )
                .build();

        _notificationManager.notify(1, notification);
    }



    private PendingIntent buildPendingIntent() {

        Intent intent = new Intent(getApplicationContext(), MainActivity.class);

        TaskStackBuilder stackBuilder = TaskStackBuilder.create(this);
        stackBuilder.addParentStack(MainActivity.class);
        stackBuilder.addNextIntent(intent);
        // Ensure a unique PendingIntents, otherwise all
        // recommendations end up with the same PendingIntent
        intent.setAction("1");

        return stackBuilder.getPendingIntent(0, PendingIntent.FLAG_UPDATE_CURRENT);
    }



    public static void scheduleRecommendationUpdate(Context context) {

        AlarmManager alarmManager = (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);
        Intent recommendationIntent = new Intent(context, RecommendationService.class);
        PendingIntent alarmIntent = PendingIntent.getService(context, 0, recommendationIntent, 0);

        alarmManager.setInexactRepeating(AlarmManager.ELAPSED_REALTIME_WAKEUP,
                INITIAL_DELAY,
                AlarmManager.INTERVAL_HOUR,
                alarmIntent);

        context.startService(recommendationIntent);
    }

}
