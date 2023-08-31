import { configureStore } from '@reduxjs/toolkit';
import userReducer from './slices/user-slice';
import alertReducer from './slices/alert-slice'

export const store = configureStore({
  reducer: {
    user: userReducer,
    alert: alertReducer
  }
})

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;