package com.markermetro.aucklandgdg;


import android.app.Application;
import android.content.Context;

public class AndroidApplication extends Application {

    public static Context AppContext;

    @Override
    public void onCreate() {
        super.onCreate();

        AppContext = getApplicationContext();

        RecommendationService.scheduleRecommendationUpdate(AppContext);
    }
}
