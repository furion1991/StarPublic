'use client'

import { useRef, useState } from 'react'
import Link from 'next/link'
import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import Image from 'next/image'

import { AccordionItem, Button, PageActions } from '@/shared/ui'
import { UpgradeDevice, useUpgradeAnimation } from '@/widgets/upgrades'
import { LootItemsSelection, useAllItems } from '@/entities/loot'
import { type UpgradeLootItem, UpgradesFAQ } from '@/entities/upgrades'
import { ItemToUpgradeFilter } from '@/features/upgrades'

import { useUser, useAuth, useShakingAnimation } from '@/shared/hooks'

import classes from './Upgrades.module.scss'

const schema = z.object({
  priceSearch: z.number(),
  nameSearch: z.string()
})
type FormSchema = z.infer<typeof schema>

export const UpgradesPage = () => {
  const [userItem, setUserItem] = useState<UpgradeLootItem | null>(null)
  const [upgradeItem, setUpgradeItem] = useState<UpgradeLootItem | null>(null)

  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })

  const { isAuth } = useAuth()
  const { user } = useUser()
  const { data: allItems, isLoading: isAllItemsLoading } = useAllItems({
    nameSearch: '',
    valueFrom: 0
  })

  const animationProps = useUpgradeAnimation()

  const linkBackRef = useRef<HTMLButtonElement>(null)
  const soundBtnRef = useRef<HTMLButtonElement>(null)
  const headingRef = useRef<HTMLHeadingElement>(null)
  const inventoryContainerRef = useRef<HTMLDivElement>(null)
  const allItemsContainerRef = useRef<HTMLDivElement>(null)
  const faqSectionRef = useRef<HTMLElement>(null)
  const itemsSelectContainerMobileRef = useRef<HTMLDivElement>(null)

  useShakingAnimation({
    elementsRef: [
      linkBackRef,
      soundBtnRef,
      headingRef,
      inventoryContainerRef,
      allItemsContainerRef,
      faqSectionRef,
      itemsSelectContainerMobileRef
    ],
    enabled: animationProps.isShaking
  })

  const userInventoryItems = user
    ? user.userInventory.availableInventoryItems.map(({ id, itemId, itemDto }) => {
        const { name, image, sellPrice, rarity } = itemDto

        return {
          inventoryId: id,
          itemId,
          name,
          image,
          sellPrice,
          rarity
        }
      })
    : []

  const allItemsFormatted = allItems
    ? allItems.map(({ id, image, rarity, name, sellPrice }) => ({
        itemId: id,
        image,
        rarity,
        name,
        sellPrice
      }))
    : []

  const userInventoryContent = (
    <div className={classes.itemsSelectContent}>
      {userInventoryItems.length ? (
        <ul className={classes.itemsSelectList}>
          <LootItemsSelection
            items={userInventoryItems}
            selectedItem={userItem}
            onItemSelect={setUserItem}
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
          items={allItemsFormatted}
          selectedItem={upgradeItem}
          onItemSelect={setUpgradeItem}
        />
      </ul>
    </div>
  )

  return (
    <>
      <PageActions
        className={classes.pageActions}
        linkBackRef={linkBackRef}
        soundBtnRef={soundBtnRef}
      />

      <div className={classes.upgradesPage}>
        <h1 ref={headingRef}>Апгрейд предметов</h1>

        <div className={classes.upgradeMain}>
          <UpgradeDevice
            itemToUpgrade={upgradeItem}
            userItem={userItem}
            animationControls={animationProps}
          />
        </div>

        <div className={classes.wrapper}>
          {isAuth ? (
            <section className={classes.itemsSelect}>
              <div ref={inventoryContainerRef} className={classes.itemsSelectContainer}>
                <div className={classes.itemsSelectHeading}>
                  <p>Ваш инвентарь</p>
                </div>

                {userInventoryContent}
              </div>

              <div ref={allItemsContainerRef} className={classes.itemsSelectContainer}>
                <div className={classes.itemsSelectHeading}>
                  <p>Предметы</p>

                  <div className={classes.itemsSearchContainer}>
                    <FormProvider {...useFormProps}>
                      <ItemToUpgradeFilter />
                    </FormProvider>
                  </div>
                </div>

                {itemsToUpgradeContent}
              </div>

              <div
                ref={itemsSelectContainerMobileRef}
                className={classes.itemsSelectContainerMobile}
              >
                <AccordionItem
                  className={classes.itemsSelectAccordionItem}
                  heading='Ваш интвентарь'
                >
                  {userInventoryContent}
                </AccordionItem>

                <AccordionItem className={classes.itemsSelectAccordionItem} heading='Предметы'>
                  <FormProvider {...useFormProps}>
                    <div className={classes.mobileFilter}>
                      <ItemToUpgradeFilter />
                    </div>
                  </FormProvider>

                  {itemsToUpgradeContent}
                </AccordionItem>
              </div>
            </section>
          ) : null}

          <section ref={faqSectionRef} className={classes.howItWorks}>
            <h2>
              <Image src='/icons/info-rounded-blue.svg' width={34} height={34} alt='инфо' />

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
