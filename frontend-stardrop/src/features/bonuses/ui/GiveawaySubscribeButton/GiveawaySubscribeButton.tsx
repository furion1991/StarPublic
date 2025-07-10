'use client'

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import { Button } from '@/shared/ui'

import { checkGiveawaySubscription, subscribeToGiveaway } from '../../api/bonuses'

export const GiveawaySubscribeButton = () => {
  const queryClient = useQueryClient()

  const { data: isSubscribed, isLoading: isSubscriptionLoading } = useQuery({
    queryFn: checkGiveawaySubscription,
    queryKey: ['giveaway-is-subscribed']
  })

  const { mutate: subscribe, isPending: isSubscribeInProcess } = useMutation({
    mutationFn: subscribeToGiveaway,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['giveaway-is-subscribed']
      })
    }
  })

  return (
    <Button
      disabled={isSubscribed}
      loading={isSubscriptionLoading || isSubscribeInProcess}
      onClick={() => {
        subscribe()
      }}
    >
      {!isSubscribed ? 'Участвовать' : 'Вы участвуете'}
    </Button>
  )
}
