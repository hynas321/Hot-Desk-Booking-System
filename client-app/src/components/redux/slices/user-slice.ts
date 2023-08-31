import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Desk } from '../../../types/Desk';

export interface User {
  username: string,
  isAdmin: boolean,
  bookedDesk: Desk | null,
  bookedDeskLocation: string | null
};

const initialState: User = {
  username: "User",
  isAdmin: false,
  bookedDesk: null,
  bookedDeskLocation: null
};

const gameSettingsSlice = createSlice({
  name: "alert",
  initialState,
  reducers: {
    updatedUsername(state, action: PayloadAction<string>) {
      state.username = action.payload;
    },
    updatedIsAdmin(state, action: PayloadAction<boolean>) {
      state.isAdmin = action.payload;
    },
    updatedBookedDesk(state, action: PayloadAction<Desk | null>) {
      state.bookedDesk = action.payload;
    },
    updatedBookedDeskLocation(state, action: PayloadAction<string | null>) {
      state.bookedDeskLocation = action.payload;
    }
  }
})

export const { updatedUsername, updatedIsAdmin, updatedBookedDesk, updatedBookedDeskLocation } = gameSettingsSlice.actions;
export default gameSettingsSlice.reducer;