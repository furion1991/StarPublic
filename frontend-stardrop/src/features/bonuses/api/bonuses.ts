import { API } from '@/shared/api'

export const claimDailyBonus = () => {
  return API.get('/daily-bonus')
}

export const subscribeToGiveaway = () => {
  return API.get('/draw/subscribe')
}

export const checkGiveawaySubscription = async () => {
  const { data } = await API.get<boolean>('/draw/is-subscribed')

  return data
}
