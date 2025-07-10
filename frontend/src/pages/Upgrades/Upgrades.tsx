'use client'

import { useState } from 'react'
import Link from 'next/link'
import { FormProvider, useForm } from 'react-hook-form'

import { AccordionItem, Button, PageActions, TextField } from '@/shared/ui'
import { UpgradeDevice } from '@/widgets/upgrades'
import { LootItemsSelection } from '@/entities/loot'
import { UpgradesFAQ } from '@/entities/upgrades'

import { useUser, useAuth } from '@/shared/hooks'

import classes from './Upgrades.module.scss'
import Image from 'next/image'

type Item = {
  id: string
  name: string
  rarity: number
  sellPrice: number
  image: string
}

export const UpgradesPage = () => {
  const [myItemToUpgrade, setMyItemToUpgrade] = useState<Item | null>(null)
  const [upgradeItem, setUpgradeItem] = useState<Item | null>(null)

  const useFormProps = useForm()

  const { isAuth } = useAuth()
  const { user } = useUser()

  const userInventoryItems = user
    ? user.userInventory.itemsUserInventory.map(({ id, itemDto }) => {
        const { name, image, sellPrice, rarity } = itemDto

        return {
          id,
          name,
          image,
          sellPrice,
          rarity
        }
      })
    : []

  const userInventoryContent = (
    <div className={classes.itemsSelectContent}>
      {userInventoryItems.length ? (
        <ul className={classes.itemsSelectList}>
          <LootItemsSelection
            items={userInventoryItems}
            selectedItem={myItemToUpgrade}
            onItemSelect={setMyItemToUpgrade}
          />
        </ul>
      ) : (
        <div className={classes.noInventoryItems}>
          <p>У вас пока нет доступных предметов. Открывайте кейсы и апгрейдите предмет</p>

          <Link href='/'>
            <Button color='purple' borderRadius='medium'>
              Открыть кейсы
            </Button>
          </Link>
        </div>
      )}
    </div>
  )

  const itemsToUpgradeContent = (
    <div className={classes.itemsSelectContent}>
      <ul className={classes.itemsSelectList}>
        <LootItemsSelection
          items={userInventoryItems}
          selectedItem={upgradeItem}
          onItemSelect={setUpgradeItem}
        />
      </ul>
    </div>
  )

  return (
    <>
      <PageActions className={classes.pageActions} />

      <div className={classes.upgradesPage}>
        <h1>Апгрейд предметов</h1>

        <div className={classes.upgradeMain}>
          <UpgradeDevice itemToUpgrade={upgradeItem} upgradeItem={myItemToUpgrade} />
        </div>

        <div className={classes.wrapper}>
          {isAuth ? (
            <section className={classes.itemsSelect}>
              <div className={classes.itemsSelectContainer}>
                <div className={classes.itemsSelectHeading}>
                  <p>Ваш инвентарь</p>
                </div>

                {userInventoryContent}
              </div>

              <div className={classes.itemsSelectContainer}>
                <div className={classes.itemsSelectHeading}>
                  <p>Предметы</p>

                  <div className={classes.itemsSearchContainer}>
                    <FormProvider {...useFormProps}>
                      <TextField
                        className={classes.itemsSearchField}
                        name='priceSearch'
                        placeholder='Цена от'
                        endAdornment={<span className={classes.endAdornment}>₽</span>}
                      />

                      <TextField
                        className={classes.itemsSearchField}
                        name='nameSearch'
                        placeholder='Поиск по названию'
                        endAdornment={
                          <span className={classes.endAdornment}>
                            <Image src='/icons/loupe.svg' width={17} height={17} alt='Лупа' />
                          </span>
                        }
                      />
                    </FormProvider>
                  </div>
                </div>

                {itemsToUpgradeContent}
              </div>

              <AccordionItem
                className={classes.itemsSelectContainerMobile}
                openClassName={classes.itemsSelectContainerMobileOpened}
                heading='Интвентарь'
              >
                {userInventoryContent}
              </AccordionItem>

              <AccordionItem
                className={classes.itemsSelectContainerMobile}
                openClassName={classes.itemsSelectContainerMobileOpened}
                heading='Интвентарь'
              >
                {itemsToUpgradeContent}
              </AccordionItem>
            </section>
          ) : null}

          <section className={classes.howItWorks}>
            <h2>
              <svg
                width='34'
                height='34'
                viewBox='0 0 20 20'
                fill='none'
                xmlns='http://www.w3.org/2000/svg'
              >
                <g id='mingcute:question-line'>
                  <g id='Group'>
                    <path
                      id='Vector'
                      d='M9.99935 1.66699C14.6018 1.66699 18.3327 5.39783 18.3327 10.0003C18.3327 14.6028 14.6018 18.3337 9.99935 18.3337C5.39685 18.3337 1.66602 14.6028 1.66602 10.0003C1.66602 5.39783 5.39685 1.66699 9.99935 1.66699ZM9.99935 3.33366C8.23124 3.33366 6.53555 4.03604 5.2853 5.28628C4.03506 6.53652 3.33268 8.23222 3.33268 10.0003C3.33268 11.7684 4.03506 13.4641 5.2853 14.7144C6.53555 15.9646 8.23124 16.667 9.99935 16.667C11.7675 16.667 13.4632 15.9646 14.7134 14.7144C15.9636 13.4641 16.666 11.7684 16.666 10.0003C16.666 8.23222 15.9636 6.53652 14.7134 5.28628C13.4632 4.03604 11.7675 3.33366 9.99935 3.33366ZM9.99935 13.3337C10.2204 13.3337 10.4323 13.4215 10.5886 13.5777C10.7449 13.734 10.8327 13.946 10.8327 14.167C10.8327 14.388 10.7449 14.6 10.5886 14.7562C10.4323 14.9125 10.2204 15.0003 9.99935 15.0003C9.77833 15.0003 9.56637 14.9125 9.41009 14.7562C9.25381 14.6 9.16602 14.388 9.16602 14.167C9.16602 13.946 9.25381 13.734 9.41009 13.5777C9.56637 13.4215 9.77833 13.3337 9.99935 13.3337ZM9.99935 5.41699C10.7012 5.41701 11.3812 5.66145 11.9225 6.10833C12.4638 6.5552 12.8325 7.17661 12.9654 7.86581C13.0983 8.55501 12.9871 9.26898 12.6508 9.88508C12.3146 10.5012 11.7742 10.981 11.1227 11.242C11.0262 11.2775 10.9392 11.3348 10.8685 11.4095C10.8318 11.4512 10.826 11.5045 10.8268 11.5595L10.8327 11.667C10.8324 11.8794 10.7511 12.0837 10.6053 12.2381C10.4595 12.3926 10.2602 12.4855 10.0482 12.498C9.83615 12.5104 9.62737 12.4414 9.46449 12.3051C9.30161 12.1688 9.19694 11.9754 9.17185 11.7645L9.16602 11.667V11.4587C9.16602 10.4978 9.94102 9.92116 10.5027 9.69532C10.7313 9.60404 10.9307 9.45221 11.0795 9.25615C11.2283 9.06008 11.3209 8.82718 11.3473 8.58246C11.3738 8.33773 11.3331 8.09043 11.2295 7.86711C11.126 7.64379 10.9636 7.45288 10.7598 7.31489C10.556 7.1769 10.3184 7.09704 10.0726 7.08388C9.82681 7.07073 9.58207 7.12477 9.36468 7.24022C9.14728 7.35566 8.96544 7.52814 8.83868 7.73914C8.71192 7.95014 8.64502 8.19168 8.64518 8.43783C8.64518 8.65884 8.55738 8.8708 8.4011 9.02708C8.24482 9.18336 8.03286 9.27116 7.81185 9.27116C7.59084 9.27116 7.37887 9.18336 7.22259 9.02708C7.06631 8.8708 6.97852 8.65884 6.97852 8.43783C6.97852 7.63665 7.29678 6.86829 7.8633 6.30177C8.42981 5.73526 9.19817 5.41699 9.99935 5.41699Z'
                      fill='#4266EA'
                    />
                  </g>
                </g>
              </svg>

              <span>Как это работает?</span>
            </h2>

            <div className={classes.howItWorksAccordionList}>
              <UpgradesFAQ />
            </div>
          </section>
        </div>
      </div>
    </>
  )
}
