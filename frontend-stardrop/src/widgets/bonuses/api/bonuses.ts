import { API } from '@/shared/api'

import type { ClaimBonusForSubscriptionProps } from './bonuses.types'

export const claimBonusForSubscription = ({
  provider,
  data,
  vkData
}: ClaimBonusForSubscriptionProps) => {
  return API.post('/subscription-bonus', {
    provider,
    data,
    vkData
  })
}
